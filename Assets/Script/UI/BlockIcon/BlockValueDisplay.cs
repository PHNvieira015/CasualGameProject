using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlockValueDisplay : MonoBehaviour, IBlockValueDisplay
{
    [Header("References")]
    [SerializeField] private Image _blockIcon;
    [SerializeField] private TMP_Text _blockText;

    [Header("Settings")]
    [SerializeField] private string _formatString = "{0}"; // Customize how the number appears
    //[SerializeField] private bool _debugMode = true;

    private Unit _boundUnit;

    private void Awake()
    {
        InitializeBlockDisplay();
    }

    public void BindToUnit(Unit unit)
    {
        _boundUnit = unit;
        // Initial update
        UpdateBlockDisplay(_boundUnit.GetStatValue(StatType.Block));

        //if (_debugMode) Debug.Log($"Bound to unit: {unit.name}");
    }

    private void InitializeBlockDisplay()
    {
        if (_blockIcon == null || _blockText == null)
        {
            //Debug.LogError("BlockValueDisplay references not set!", this);
            return;
        }

        // Start hidden
        gameObject.SetActive(false);

        //if (_debugMode) Debug.Log("BlockValueDisplay initialized");
    }

    public void UpdateBlockDisplay(int currentBlockValue)
    {
        if (_blockIcon == null || _blockText == null)
        {
            //Debug.LogWarning("BlockValueDisplay references missing during update!", this);
            return;
        }

        // Handle visibility
        bool shouldShow = currentBlockValue > 0;
        gameObject.SetActive(shouldShow);

        if (!shouldShow) return;

        // Update text
        _blockText.SetText(string.Format(_formatString, currentBlockValue));

        //if (_debugMode) Debug.Log($"Block value updated: {currentBlockValue}");
    }
}