using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffDebuffIcon : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _stacksText;
    [SerializeField] private GameObject _stacksBackground;

    public string EffectName { get; private set; }
    public int CurrentStacks { get; private set; }

    // FIXED: Simplified Initialize method with correct parameters
    public void Initialize(string effectName, int stacks)
    {
        EffectName = effectName;
        CurrentStacks = stacks;
        UpdateStacksDisplay();

        Debug.Log($"Initialized icon for {effectName}");
    }

    public void UpdateStacks(int stacks)
    {
        CurrentStacks = stacks;
        UpdateStacksDisplay();
    }

    private void UpdateStacksDisplay()
    {
        if (_stacksText != null)
        {
            _stacksText.text = CurrentStacks > 1 ? CurrentStacks.ToString() : "";

            if (_stacksBackground != null)
            {
                _stacksBackground.SetActive(CurrentStacks > 1);
            }
        }
    }
}