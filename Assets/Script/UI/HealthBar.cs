using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour, IHealthBar
{
    [Header("References")]
    [SerializeField] private RectTransform _barRect;
    [SerializeField] private RectMask2D _mask;
    [SerializeField] private TMP_Text _hpIndicator;

    [Header("Settings")]
    [SerializeField] private bool _debugMode = true;

    private float _maxRightMask;
    private float _initialRightMask;
    private Unit _boundUnit;

    private void Awake()
    {
        InitializeHealthBar();
    }

    public void BindToUnit(Unit unit)
    {
        _boundUnit = unit;
        // Initial update
        UpdateHealthDisplay(
            _boundUnit.GetStatValue(StatType.HP),
            _boundUnit.GetStatValue(StatType.MaxHP)
        );

        if (_debugMode) Debug.Log($"Bound to unit: {unit.name}");
    }

    private void InitializeHealthBar()
    {
        if (_barRect == null || _mask == null || _hpIndicator == null)
        {
            Debug.LogError("HealthBar references not set!", this);
            return;
        }

        _maxRightMask = _barRect.rect.width - _mask.padding.x - _mask.padding.z;
        _initialRightMask = _mask.padding.z;

        if (_debugMode) Debug.Log("HealthBar initialized");
    }

    public void UpdateHealthDisplay(int currentHP, int maxHP)
    {
        if (_mask == null || _barRect == null || _hpIndicator == null)
        {
            Debug.LogWarning("HealthBar references missing during update!", this);
            return;
        }

        // Safety checks
        if (maxHP <= 0) maxHP = 1;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        // Calculate health percentage
        float healthPercentage = (float)currentHP / maxHP;
        float targetWidth = _maxRightMask * healthPercentage;
        float newRightMask = _maxRightMask + _initialRightMask - targetWidth;

        // Apply padding
        var padding = _mask.padding;
        padding.z = newRightMask;
        _mask.padding = padding;

        // Update text
        _hpIndicator.SetText($"{currentHP}/{maxHP}");

        if (_debugMode) Debug.Log($"Health updated: {currentHP}/{maxHP} ({healthPercentage:P0})");
    }
}