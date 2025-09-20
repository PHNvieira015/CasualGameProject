using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlockValueDisplay : MonoBehaviour, IBlockValueDisplay
{
    [Header("References")]
    [SerializeField] private Image _blockIcon;
    [SerializeField] private TMP_Text _blockText;

    [Header("Settings")]
    [SerializeField] private string _formatString = "{0}";
    [SerializeField] private bool _debugMode = true; // Enable this for debugging

    private Unit _boundUnit;

    private void Awake()
    {
        // REMOVED: gameObject.SetActive(false);
        // Just do validation, don't change active state
        if (_blockIcon == null || _blockText == null)
        {
            if (_debugMode) Debug.LogError("BlockValueDisplay references not set!", this);
        }

        if (_debugMode) Debug.Log("BlockValueDisplay initialized");
    }

    public void BindToUnit(Unit unit)
    {
        _boundUnit = unit;
        // Initial update
        UpdateBlockDisplay(_boundUnit.GetStatValue(StatType.Block));

        if (_debugMode) Debug.Log($"Bound to unit: {unit.name}");
    }

    public void UpdateBlockDisplay(int currentBlockValue)
    {
        if (_blockIcon == null || _blockText == null)
        {
            if (_debugMode) Debug.LogWarning("BlockValueDisplay references missing during update!", this);
            return;
        }

        // Handle visibility - control the entire GameObject
        bool shouldShow = currentBlockValue > 0;
        gameObject.SetActive(shouldShow);

        if (_debugMode) Debug.Log($"Setting block display active: {shouldShow} (Value: {currentBlockValue})");

        if (!shouldShow) return;

        // Update text
        _blockText.SetText(string.Format(_formatString, currentBlockValue));

        if (_debugMode) Debug.Log($"Block value updated: {currentBlockValue}");
    }
}