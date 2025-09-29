using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoneyRewardButton : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] private Button rewardButton;
    [SerializeField] private TextMeshProUGUI buttonText;

    [Header("UI Settings")]
    [SerializeField] private string buttonFormat = "Claim {0} Gold";
    [SerializeField] private string claimedText = "Reward Claimed!";

    private bool _rewardClaimed = false;
    private int _pendingReward = 0;

    private void Start()
    {
        if (rewardButton == null)
            rewardButton = GetComponent<Button>();

        if (buttonText == null)
            buttonText = GetComponentInChildren<TextMeshProUGUI>();

        if (rewardButton != null)
        {
            rewardButton.onClick.AddListener(ClaimReward);
        }

        UpdateButtonState();
    }

    private void OnEnable()
    {
        // When the button becomes active (when reward screen opens), calculate the reward
        CalculatePendingReward();
    }

    public void CalculatePendingReward()
    {
        if (EncounterManager.Instance == null)
        {
            Debug.LogError("EncounterManager not found!");
            _pendingReward = 0;
            UpdateButtonState();
            return;
        }

        // SIMPLE: Just ask the EncounterManager what the reward should be
        _pendingReward = EncounterManager.Instance.CalculateBattleReward();
        _rewardClaimed = false;

        Debug.Log($"[MoneyRewardButton] Reward calculated: {_pendingReward} gold");
        UpdateButtonState();
    }

    public void ClaimReward()
    {
        if (_rewardClaimed)
        {
            Debug.LogWarning("Reward already claimed!");
            return;
        }

        if (MoneyKeySystem.Instance == null)
        {
            Debug.LogError("MoneyKeySystem not found!");
            return;
        }

        if (_pendingReward <= 0)
        {
            Debug.LogWarning("No pending reward to claim!");
            return;
        }

        // Add the money to player's balance
        if (MoneyKeySystem.Instance.AddMoney(_pendingReward))
        {
            _rewardClaimed = true;
            UpdateButtonState();

            Debug.Log($"[MoneyRewardButton] Claimed {_pendingReward} gold! New balance: {MoneyKeySystem.Instance.Money}");
        }
        else
        {
            Debug.LogError("Failed to add money reward!");
        }
    }

    private void UpdateButtonState()
    {
        if (rewardButton == null || buttonText == null) return;

        if (_rewardClaimed)
        {
            rewardButton.interactable = false;
            buttonText.text = claimedText;
        }
        else
        {
            rewardButton.interactable = true;
            buttonText.text = string.Format(buttonFormat, _pendingReward);
        }
    }

    // Reset for new encounters
    public void ResetForNewEncounter()
    {
        _rewardClaimed = false;
        _pendingReward = 0;
        UpdateButtonState();
    }

    private void OnDestroy()
    {
        if (rewardButton != null)
            rewardButton.onClick.RemoveListener(ClaimReward);
    }

    // Debug method to test if it's working
    [ContextMenu("Test Reward Calculation")]
    public void TestRewardCalculation()
    {
        CalculatePendingReward();
        Debug.Log($"Pending reward: {_pendingReward}");
    }
}