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

    // Public method for Unit class to call directly
    public void AddBuffDebuff(string effectName, int value)
    {
        if (_activeIcons.ContainsKey(effectName))
        {
            _activeIcons[effectName].UpdateStacks(value);
        }
        else
        {
            AddBuffDebuffIcon(effectName, value);
        }
    }

    public void RefreshUI()
    {
        // Don't clear all - just update existing icons and add new ones
        if (_owner != null)
        {
            // Track which effects we found in this refresh
            HashSet<string> foundEffects = new HashSet<string>();

            // Update existing icons or create new ones
            StatusEffect[] statusEffects = _owner.GetComponentsInChildren<StatusEffect>(true);
            foreach (StatusEffect effect in statusEffects)
            {
                if (effect != null && effect.isActiveAndEnabled)
                {
                    string effectName = GetEffectName(effect);
                    int stacks = GetStacks(effect);
                    foundEffects.Add(effectName);

                    // Update existing icon or create new one
                    if (_activeIcons.ContainsKey(effectName))
                    {
                        _activeIcons[effectName].UpdateStacks(stacks);
                    }
                    else
                    {
                        AddBuffDebuffIcon(effectName, stacks);
                    }
                }
            }

            // Also check Buff_Debuff tagged objects
            foreach (Transform child in _owner.transform)
            {
                if (child.CompareTag("Buff_Debuff"))
                {
                    string effectName = GetEffectNameFromObject(child.gameObject);
                    int stacks = GetStacksFromObject(child.gameObject);
                    foundEffects.Add(effectName);

                    if (_activeIcons.ContainsKey(effectName))
                    {
                        _activeIcons[effectName].UpdateStacks(stacks);
                    }
                    else
                    {
                        AddBuffDebuffIcon(effectName, stacks);
                    }
                }
            }

            // Remove icons for effects that no longer exist
            List<string> effectsToRemove = new List<string>();
            foreach (var effectName in _activeIcons.Keys)
            {
                if (!foundEffects.Contains(effectName))
                {
                    effectsToRemove.Add(effectName);
                }
            }

            foreach (string effectName in effectsToRemove)
            {
                RemoveBuffDebuff(effectName);
            }
        }
    }

    private void AddBuffDebuffIcon(string effectName, int stacks)
    {
        GameObject iconPrefab = GetIconPrefabForEffect(effectName);
        if (iconPrefab == null) return;

        GameObject iconObject = Instantiate(iconPrefab, _buffDebuffContainer);
        iconObject.SetActive(true);

        BuffDebuffIcon iconComponent = iconObject.GetComponent<BuffDebuffIcon>();

        if (iconComponent != null)
        {
            iconComponent.Initialize(effectName, stacks);
            _activeIcons[effectName] = iconComponent;
            UpdateIconPositions();
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
            var icon = _activeIcons[effectName];
            if (icon != null && icon.gameObject != null)
            {
                Destroy(icon.gameObject);
            }
            _activeIcons.Remove(effectName);
            UpdateIconPositions();
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
        // Just clear the dictionary - let Unity handle the GameObject destruction automatically
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

    private int GetStacks(StatusEffect effect)
    {
        // For duration-based effects, show remaining turns
        if (effect.StacksDuration)
        {
            return effect.CurrentDuration; // Use the public property
        }

        // For intensity-based effects, show intensity
        if (effect.StacksIntensity)
        {
            return effect.CurrentAmount; // Use the public property
        }

        // Default: show 1 if active
        return 1;
    }

    private int GetStacksFromObject(GameObject buffDebuffObj)
    {
        StatusEffect statusEffect = buffDebuffObj.GetComponent<StatusEffect>();
        if (statusEffect != null)
        {
            return GetStacks(statusEffect);
        }

        // Default to 1 if no StatusEffect component
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

    // MESSAGE METHOD - COMMENTED OUT
    /*
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
    */

    // DEBUFF CHECK METHOD - COMMENTED OUT (but kept for future use)
    /*
    private bool IsDebuff(string effectName)
    {
        return effectName.ToLower().Contains("weak") ||
               effectName.ToLower().Contains("vulnerable1") ||
               effectName.ToLower().Contains("vulnerable2") ||
               effectName.ToLower().Contains("frail") ||
               effectName.ToLower().Contains("poison");
    }
    */
}