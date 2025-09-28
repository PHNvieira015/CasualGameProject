using UnityEngine;
using System.Collections.Generic;

public class BuffDebuffHolder : MonoBehaviour
{
    [SerializeField] private Transform _buffDebuffContainer;

    // Add a list that maps effect names to specific icon prefabs
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
        // Create container if it doesn't exist
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
        RefreshUI(); // Initial refresh
    }

    public void RefreshUI()
    {
        // Clear existing icons
        ClearAll();

        // Find all buff/debuff objects on the unit
        if (_owner != null)
        {
            // Find StatusEffect components
            StatusEffect[] statusEffects = _owner.GetComponentsInChildren<StatusEffect>();
            foreach (StatusEffect effect in statusEffects)
            {
                if (effect != null && effect.isActiveAndEnabled)
                {
                    AddStatusEffectIcon(effect);
                }
            }

            // Also find GameObjects with Buff_Debuff tag
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
        int stacks = GetStacks(effect);

        AddBuffDebuff(effectName, stacks);
    }

    private void AddBuffDebuffObject(GameObject buffDebuffObj)
    {
        if (buffDebuffObj == null) return;

        string effectName = GetEffectNameFromObject(buffDebuffObj);
        int stacks = GetStacksFromObject(buffDebuffObj);

        AddBuffDebuff(effectName, stacks);
    }

    public void AddBuffDebuff(string effectName, int stacks)
    {
        // Find the correct icon prefab for this effect
        GameObject iconPrefab = GetIconPrefabForEffect(effectName);
        if (iconPrefab == null)
        {
            Debug.LogWarning($"No icon prefab found for effect: {effectName}");
            return;
        }

        if (_activeIcons.ContainsKey(effectName))
        {
            // Update existing icon stacks
            _activeIcons[effectName].UpdateStacks(stacks);
        }
        else
        {
            // Create new icon using the specific prefab
            GameObject iconObject = Instantiate(iconPrefab, _buffDebuffContainer);
            BuffDebuffIcon iconComponent = iconObject.GetComponent<BuffDebuffIcon>();

            if (iconComponent != null)
            {
                // FIXED: Call the correct Initialize method
                iconComponent.Initialize(effectName, stacks);
                _activeIcons[effectName] = iconComponent;

                // Position the icon
                UpdateIconPositions();
            }
        }

        // Spawn message
        if (_owner != null)
        {
            var spawner = _owner.GetComponent<BuffDebuffMessageSpawner>();
            if (spawner != null)
            {
                // Determine if it's a debuff for message purposes
                bool isDebuff = IsDebuff(effectName);
                if (isDebuff)
                    spawner.SpawnDebuffMessage(effectName, stacks);
                else
                    spawner.SpawnBuffMessage(effectName, stacks);
            }
        }

        Debug.Log($"Added icon for {effectName} with {stacks} stacks");
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

            // Spawn expiration message
            if (_owner != null)
            {
                // FIXED: Corrected typo in BuffDebuffMessageSpawner
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
        // For TagModifierStatusEffect, use the Tag name
        if (effect is TagModifierStatusEffect tagEffect)
        {
            return tagEffect.Tag.ToString();
        }

        // Try to get the name from the GameObject name
        string objectName = effect.gameObject.name;
        if (objectName.Contains("(Clone)"))
            objectName = objectName.Replace("(Clone)", "").Trim();
        if (objectName.Contains("("))
            objectName = objectName.Substring(0, objectName.IndexOf("(")).Trim();

        return objectName;
    }

    private string GetEffectNameFromObject(GameObject buffDebuffObj)
    {
        // Get the name from the GameObject
        string objectName = buffDebuffObj.name;

        // Clean up the name (remove clone, etc.)
        if (objectName.Contains("(Clone)"))
            objectName = objectName.Replace("(Clone)", "").Trim();
        if (objectName.Contains("("))
            objectName = objectName.Substring(0, objectName.IndexOf("(")).Trim();

        return objectName;
    }

    private int GetStacks(StatusEffect effect)
    {
        if (effect.StacksIntensity)
        {
            return effect._currentAmount;
        }

        if (effect.StacksDuration)
        {
            return effect.Duration;
        }

        return 1;
    }

    private int GetStacksFromObject(GameObject buffDebuffObj)
    {
        // Check if the object has a StatusEffect component for stacks
        StatusEffect statusEffect = buffDebuffObj.GetComponent<StatusEffect>();
        if (statusEffect != null)
        {
            if (statusEffect.StacksIntensity)
                return statusEffect._currentAmount;
            if (statusEffect.StacksDuration)
                return statusEffect.Duration;
        }

        // Default to 1 stack
        return 1;
    }

    private bool IsDebuff(string effectName)
    {
        // Define which effects are debuffs
        return effectName.ToLower().Contains("weak") ||
               effectName.ToLower().Contains("vulnerable1") ||
               effectName.ToLower().Contains("Vulnerable2") ||
               effectName.ToLower().Contains("poison") ||
               effectName.ToLower().Contains("Dexterity") ||
               effectName.ToLower().Contains("Armor") ||
               effectName.ToLower().Contains("Frail") ||
               effectName.ToLower().Contains("Heal") ||
               effectName.ToLower().Contains("Stronger!");
    }
}