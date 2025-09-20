using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public delegate void OnUnit(Unit unit);

public class Unit : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private DamageIndicatorSpawner _damageSpawner;
    [SerializeField] private BuffDebuffMessageSpawner _buffDebuffSpawner;
    public event Action<Unit, StatType, int, int> OnStatChanged;  // (unit, statType, oldValue, newValue)
    public event Action<Unit, string, int, bool> OnStatusEffectApplied; // (unit, effectName, stacks, isDebuff)

    private bool _isHealthChanging;
    private bool _isBlockAbsorbingDamage;

    [Header("Stats")]
    [SerializeField] private List<Stat> _stats;

    private IHealthBar _healthBar;
    private IBlockValueDisplay _blockDisplay;
    public OnUnit OnUnitClicked = delegate { };
    public OnUnit OnUnitTakeTurn = delegate { };
    public TagModifier[] Modify = new TagModifier[(int)ModifierTags.None];

    // Event for unit death notifications
    public static event System.Action<Unit> OnAnyUnitDeath;

    #region Initialization
    protected virtual void Awake()
    {
        InitializeStats();
        FindHealthBar();
        InitializeHealthBarConnection();
        InitializeBlockDisplayConnection();
    }

    private void InitializeStats()
    {
        if (_stats == null || _stats.Count == 0)
        {
            GenerateStats();
        }
        else
        {
            // Ensure current HP doesn't exceed MaxHP
            SetStatValue(StatType.HP, GetStatValue(StatType.HP));
        }
    }

    private void FindHealthBar()
    {
        _healthBar = GetComponentInChildren<IHealthBar>();
        if (_healthBar == null)
        {
            _healthBar = GetComponentInParent<IHealthBar>();
        }
        UpdateHealthBar(); // Initial update
    }
    #endregion

    #region Public Methods
    public void RegisterHealthBar(IHealthBar healthBar)
    {
        _healthBar = healthBar;
        UpdateHealthBar();
    }

    public void UnregisterHealthBar()
    {
        _healthBar = null;
    }

    [ContextMenu("Generate Stats")]
    public void GenerateStats()
    {
        _stats = new List<Stat>();
        for (int i = 0; i < (int)StatType.None; i++)
        {
            Stat stat = new Stat
            {
                Type = (StatType)i,
                Value = GetDefaultStatValue((StatType)i)
            };
            _stats.Add(stat);
        }

        // Set current HP to max HP initially
        SetStatValue(StatType.HP, GetStatValue(StatType.MaxHP));
    }

    public int GetStatValue(StatType type)
    {
        if (_stats == null || _stats.Count <= (int)type)
            return 0;

        return _stats[(int)type].Value;
    }

    public void SetStatValue(StatType type, int value)
    {
        if (_stats == null || _stats.Count <= (int)type) return;

        int oldValue = _stats[(int)type].Value;
        value = ProcessStatValue(type, value);
        _stats[(int)type].Value = value;

        // Handle messages and health bar updates
        if (IsHealthStat(type) || type == StatType.Block)
        {
            SpawnStatChangeMessage(type, oldValue, value);
        }

        if (IsHealthStat(type))
        {
            UpdateHealthBar();

            if (type == StatType.HP && value <= 0)
            {
                DestroyUnit();
            }
        }
        else if (type == StatType.Block)
        {
            UpdateBlockDisplay();
        }
    }

    public virtual IEnumerator Recover()
    {
        yield return null;
        SetStatValue(StatType.Block, 0);
        OnUnitTakeTurn(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnUnitClicked(this);
    }
    #endregion

    #region Helper Methods
    private int GetDefaultStatValue(StatType type)
    {
        switch (type)
        {
            case StatType.MaxHP: return 100;
            case StatType.HP: return 100;
            case StatType.Block: return 0;
            default: return 0;
        }
    }

    private int ProcessStatValue(StatType type, int value)
    {
        switch (type)
        {
            case StatType.HP:
                return Mathf.Clamp(value, 0, GetStatValue(StatType.MaxHP));

            case StatType.MaxHP:
                // If lowering MaxHP, also lower current HP if needed
                if (value < GetStatValue(StatType.HP))
                {
                    SetStatValue(StatType.HP, value);
                }
                return Mathf.Max(1, value); // Ensure at least 1 MaxHP

            case StatType.Block:
                return Mathf.Max(0, value);

            default:
                return value;
        }
    }

    private bool IsHealthStat(StatType type)
    {
        return type == StatType.HP || type == StatType.MaxHP;
    }

    private void UpdateHealthBar()
    {
        _healthBar?.UpdateHealthDisplay(
            GetStatValue(StatType.HP),
            GetStatValue(StatType.MaxHP)
        );
    }

    private void UpdateBlockDisplay()
    {
        _blockDisplay?.UpdateBlockDisplay(GetStatValue(StatType.Block));
    }
    #endregion

    private void InitializeHealthBarConnection()
    {
        // Try to find health bar in children
        _healthBar = GetComponentInChildren<IHealthBar>(true);

        if (_healthBar != null)
        {
            // If using the BindToUnit approach
            if (_healthBar is HealthBar healthBarComponent)
            {
                healthBarComponent.BindToUnit(this);
            }

            // Initial update
            _healthBar.UpdateHealthDisplay(
                GetStatValue(StatType.HP),
                GetStatValue(StatType.MaxHP)
            );
        }
        else
        {
            //Debug.LogWarning($"No IHealthBar found for {gameObject.name}", this);
        }
    }

    private void InitializeBlockDisplayConnection()
    {
        // Try to find block display in children
        _blockDisplay = GetComponentInChildren<IBlockValueDisplay>(true);

        if (_blockDisplay != null)
        {
            // If using the BindToUnit approach
            if (_blockDisplay is BlockValueDisplay blockDisplayComponent)
            {
                blockDisplayComponent.BindToUnit(this);
            }
            else if (_blockDisplay is MonoBehaviour blockMonoBehaviour)
            {
                // Alternative: check if the component has a BindToUnit method
                var bindMethod = blockMonoBehaviour.GetType().GetMethod("BindToUnit");
                if (bindMethod != null)
                {
                    bindMethod.Invoke(blockMonoBehaviour, new object[] { this });
                }
            }

            // Initial update
            UpdateBlockDisplay();
        }
        else
        {
            Debug.LogWarning($"No IBlockValueDisplay found for {gameObject.name}", this);
        }
    }

    public void RegisterBlockDisplay(IBlockValueDisplay blockDisplay)
    {
        _blockDisplay = blockDisplay;
        UpdateBlockDisplay();
    }

    public void UnregisterBlockDisplay()
    {
        _blockDisplay = null;
    }

    private void SpawnStatChangeMessage(StatType type, int oldValue, int newValue)
    {
        if (newValue == oldValue || _damageSpawner == null) return;

        int change = newValue - oldValue;

        // 1. ALWAYS show health changes
        if (type == StatType.HP)
        {
            bool wasBlocked = (change < 0) && (GetStatValue(StatType.Block) > 0);
            _damageSpawner.SpawnMessage(new HealthChangeArgs(
                oldValue, newValue, change,
                StatType.HP, wasBlocked
            ));
        }
        // 2. Only show block changes if:
        //    - It's a gain OR
        //    - We still have block remaining
        else if (type == StatType.Block && (change > 0 || newValue > 0))
        {
            _damageSpawner.SpawnMessage(new HealthChangeArgs(
                oldValue, newValue, change,
                StatType.Block, false
            ));
        }
    }

    private void DestroyUnit()
    {
        // Notify that a unit died (for victory/defeat checking)
        OnAnyUnitDeath?.Invoke(this);

        // Trigger any death events or animations first
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        Debug.Log($"{gameObject.name} has died");

        // Optional: Play death animation
        // GetComponent<Animator>()?.SetTrigger("Die");

        // Optional: Wait for animation to complete
        yield return new WaitForSeconds(0.5f);

        // Destroy the object
        Destroy(gameObject);
    }

    private IEnumerator FadeOut()
    {
        // Example fade out effect
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            float fadeTime = 0.5f;
            float elapsed = 0f;
            Color originalColor = renderer.material.color;

            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
                renderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }
        }
    }
}