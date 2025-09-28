using UnityEngine;
using System.Collections.Generic;

public class BuffDebuffHolder : MonoBehaviour
{
    [SerializeField] private Transform _buffDebuffContainer;

    [System.Serializable]
    public class EffectIconMapping
    {
        public string effectName;
        public GameObject iconPrefab;
    }

    [SerializeField] private List<EffectIconMapping> _effectIconMappings = new List<EffectIconMapping>();

    private Dictionary<string, BuffDebuffIcon> _activeIcons = new Dictionary<string, BuffDebuffIcon>();
    private Unit _owner;

    void Awake()
    {
        if (_buffDebuffContainer == null)
        {
            GameObject container = new GameObject("BuffDebuffContainer");
            container.transform.SetParent(transform);
            container.transform.localPosition = Vector3.zero;
            _buffDebuffContainer = container.transform;
        }
    }

    public void Initialize(Unit owner)
    {
        _owner = owner;
        RefreshUI();
    }

    public void RefreshUI()
    {
        ClearAll();

        if (_owner != null)
        {
            StatusEffect[] statusEffects = _owner.GetComponentsInChildren<StatusEffect>(true);
            foreach (StatusEffect effect in statusEffects)
            {
                if (effect != null && effect.isActiveAndEnabled)
                {
                    AddStatusEffectIcon(effect);
                }
            }

            foreach (Transform child in _owner.transform)
            {
                if (child.CompareTag("Buff_Debuff"))
                {
                    AddBuffDebuffObject(child.gameObject);
                }
            }
        }
    }

    private void AddStatusEffectIcon(StatusEffect effect)
    {
        if (effect == null) return;

        string effectName = GetEffectName(effect);
        int displayValue = GetDisplayValue(effect);
        bool isPercentage = IsPercentageEffect(effect);

        AddBuffDebuff(effectName, displayValue, isPercentage);
    }

    private void AddBuffDebuffObject(GameObject buffDebuffObj)
    {
        if (buffDebuffObj == null) return;

        string effectName = GetEffectNameFromObject(buffDebuffObj);
        int displayValue = GetDisplayValueFromObject(buffDebuffObj);
        bool isPercentage = IsPercentageEffectFromObject(buffDebuffObj);

        AddBuffDebuff(effectName, displayValue, isPercentage);
    }

    public void AddBuffDebuff(string effectName, int value, bool isPercentage = false)
    {
        GameObject iconPrefab = GetIconPrefabForEffect(effectName);
        if (iconPrefab == null) return;

        if (_activeIcons.ContainsKey(effectName))
        {
            _activeIcons[effectName].UpdateStacks(value);
        }
        else
        {
            GameObject iconObject = Instantiate(iconPrefab, _buffDebuffContainer);

            // ACTIVATE THE ICON OBJECT
            iconObject.SetActive(true);

            BuffDebuffIcon iconComponent = iconObject.GetComponent<BuffDebuffIcon>();

            if (iconComponent != null)
            {
                iconComponent.Initialize(effectName, value);
                _activeIcons[effectName] = iconComponent;
                UpdateIconPositions();

                Debug.Log("Icon created and activated for: " + effectName);
            }
            else
            {
                Debug.LogError("BuffDebuffIcon component missing on prefab: " + iconPrefab.name);
            }
        }

        if (_owner != null)
        {
            var spawner = _owner.GetComponent<BuffDebuffMessageSpawner>();
            if (spawner != null)
            {
                bool isDebuff = IsDebuff(effectName);
                string message = GetEffectMessage(effectName, value, isPercentage);

                if (isDebuff)
                    spawner.SpawnDebuffMessage(message, 0);
                else
                    spawner.SpawnBuffMessage(message, 0);
            }
        }
    }

    private string GetEffectMessage(string effectName, int value, bool isPercentage)
    {
        if (isPercentage)
        {
            switch (value)
            {
                case 200:
                    if (effectName.ToLower().Contains("strength"))
                        return "Double Strength";
                    else if (effectName.ToLower().Contains("dexterity"))
                        return "Double Dexterity";
                    else
                        return "Double " + effectName;
                default:
                    return "+" + value + "% " + effectName;
            }
        }
        else
        {
            return "+" + value + " " + effectName;
        }
    }

    private GameObject GetIconPrefabForEffect(string effectName)
    {
        foreach (var mapping in _effectIconMappings)
        {
            if (mapping.effectName == effectName)
            {
                return mapping.iconPrefab;
            }
        }
        return null;
    }

    public void RemoveBuffDebuff(string effectName)
    {
        if (_activeIcons.ContainsKey(effectName))
        {
            Destroy(_activeIcons[effectName].gameObject);
            _activeIcons.Remove(effectName);
            UpdateIconPositions();

            if (_owner != null)
            {
                var spawner = _owner.GetComponent<BuffDebuffMessageSpawner>();
                if (spawner != null)
                {
                    spawner.SpawnExpirationMessage(effectName);
                }
            }
        }
    }

    public void UpdateStacks(string effectName, int stacks)
    {
        if (_activeIcons.ContainsKey(effectName))
        {
            if (stacks <= 0)
            {
                RemoveBuffDebuff(effectName);
            }
            else
            {
                _activeIcons[effectName].UpdateStacks(stacks);
            }
        }
    }

    private void UpdateIconPositions()
    {
        int index = 0;
        float spacing = 60f;

        foreach (var kvp in _activeIcons)
        {
            if (kvp.Value != null)
            {
                RectTransform rect = kvp.Value.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.anchoredPosition = new Vector2(index * spacing, 0);
                    index++;
                }
            }
        }
    }

    public void ClearAll()
    {
        foreach (var icon in _activeIcons.Values)
        {
            if (icon != null)
            {
                Destroy(icon.gameObject);
            }
        }
        _activeIcons.Clear();
    }

    public bool HasEffect(string effectName)
    {
        return _activeIcons.ContainsKey(effectName);
    }

    public int GetStacks(string effectName)
    {
        if (_activeIcons.ContainsKey(effectName))
        {
            return _activeIcons[effectName].CurrentStacks;
        }
        return 0;
    }

    public BuffDebuffIcon GetIcon(string effectName)
    {
        if (_activeIcons.ContainsKey(effectName))
        {
            return _activeIcons[effectName];
        }
        return null;
    }

    private string GetEffectName(StatusEffect effect)
    {
        if (effect is TagModifierStatusEffect tagEffect)
        {
            return tagEffect.Tag.ToString();
        }

        string objectName = effect.gameObject.name;
        if (objectName.Contains("(Clone)"))
            objectName = objectName.Replace("(Clone)", "").Trim();
        if (objectName.Contains("("))
            objectName = objectName.Substring(0, objectName.IndexOf("(")).Trim();

        return objectName;
    }

    private string GetEffectNameFromObject(GameObject buffDebuffObj)
    {
        string objectName = buffDebuffObj.name;

        if (objectName.Contains("(Clone)"))
            objectName = objectName.Replace("(Clone)", "").Trim();
        if (objectName.Contains("("))
            objectName = objectName.Substring(0, objectName.IndexOf("(")).Trim();

        return objectName;
    }

    private int GetDisplayValue(StatusEffect effect)
    {
        if (effect is TagModifierStatusEffect tagEffect)
        {
            return tagEffect.AppliedValue;
        }
        return effect.Amount;
    }

    private int GetDisplayValueFromObject(GameObject buffDebuffObj)
    {
        StatusEffect statusEffect = buffDebuffObj.GetComponent<StatusEffect>();
        if (statusEffect != null)
        {
            return GetDisplayValue(statusEffect);
        }
        return 1;
    }

    private bool IsPercentageEffect(StatusEffect effect)
    {
        if (effect is TagModifierStatusEffect tagEffect)
        {
            return tagEffect.isPercentage;
        }
        return false;
    }

    private bool IsPercentageEffectFromObject(GameObject buffDebuffObj)
    {
        TagModifierStatusEffect tagEffect = buffDebuffObj.GetComponent<TagModifierStatusEffect>();
        if (tagEffect != null)
        {
            return tagEffect.isPercentage;
        }
        return false;
    }

    private bool IsDebuff(string effectName)
    {
        return effectName.ToLower().Contains("weak") ||
               effectName.ToLower().Contains("vulnerable1") ||
               effectName.ToLower().Contains("vulnerable2") ||
               effectName.ToLower().Contains("frail") ||
               effectName.ToLower().Contains("poison");
    }
    [ContextMenu("Debug Check Icon Setup")]
    public void DebugCheckIconSetup()
    {
        Debug.Log("=== BUFF/DEBUFF HOLDER DEBUG ===");
        Debug.Log("Owner: " + (_owner != null ? _owner.name : "null"));
        Debug.Log("Effect Icon Mappings: " + _effectIconMappings.Count);

        foreach (var mapping in _effectIconMappings)
        {
            Debug.Log("Mapping: '" + mapping.effectName + "' -> " + (mapping.iconPrefab != null ? mapping.iconPrefab.name : "NULL"));
        }

        Debug.Log("Active Icons: " + _activeIcons.Count);
        foreach (var icon in _activeIcons)
        {
            Debug.Log("Active: " + icon.Key);
        }

        // Check if we can find any buff/debuff objects
        if (_owner != null)
        {
            int buffCount = 0;
            foreach (Transform child in _owner.transform)
            {
                if (child.CompareTag("Buff_Debuff"))
                {
                    buffCount++;
                    Debug.Log("Found Buff_Debuff: " + child.name);
                }
            }
            Debug.Log("Total Buff_Debuff objects: " + buffCount);
        }
    }
}