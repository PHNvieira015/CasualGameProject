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

        Debug.Log("[MoneyRewardButton] Start() called - Initializing button");
        UpdateButtonState();
    }

    private void OnEnable()
    {
        Debug.Log("[MoneyRewardButton] BUTTON ENABLED - REWARD SCREEN OPENED");
        CalculatePendingReward();
    }

    public void CalculatePendingReward()
    {
        Debug.Log("[MoneyRewardButton] CALCULATING PENDING REWARD");

        if (EncounterManager.Instance == null)
        {
            Debug.LogError("[MoneyRewardButton] EncounterManager not found!");
            _pendingReward = 0;
            UpdateButtonState();
            return;
        }

        Debug.Log("[MoneyRewardButton] EncounterManager found");

        EncounterType currentType = EncounterManager.Instance.GetCurrentEncounterType();
        Debug.Log("[MoneyRewardButton] Current encounter type: " + currentType);

        _pendingReward = EncounterManager.Instance.CalculateBattleReward();
        _rewardClaimed = false;

        Debug.Log("[MoneyRewardButton] Calculated reward: " + _pendingReward + " gold");
        UpdateButtonState();
    }

    public void ClaimReward()
    {
        Debug.Log("[MoneyRewardButton] CLAIM REWARD CLICKED");
        Debug.Log("[MoneyRewardButton] Pending: " + _pendingReward + ", Claimed: " + _rewardClaimed);

        if (_rewardClaimed)
        {
            Debug.LogWarning("[MoneyRewardButton] Reward already claimed!");
            return;
        }

        if (MoneyKeySystem.Instance == null)
        {
            Debug.LogError("[MoneyRewardButton] MoneyKeySystem not found!");
            return;
        }

        if (_pendingReward <= 0)
        {
            Debug.LogWarning("[MoneyRewardButton] No pending reward to claim! (" + _pendingReward + ")");
            return;
        }

        int oldMoney = MoneyKeySystem.Instance.Money;
        if (MoneyKeySystem.Instance.AddMoney(_pendingReward))
        {
            _rewardClaimed = true;
            UpdateButtonState();

            Debug.Log("[MoneyRewardButton] Successfully claimed " + _pendingReward + " gold! Old: " + oldMoney + ", New: " + MoneyKeySystem.Instance.Money);
        }
        else
        {
            Debug.LogError("[MoneyRewardButton] Failed to add money reward!");
        }
    }

    private void UpdateButtonState()
    {
        Debug.Log("[MoneyRewardButton] Updating button state - Reward: " + _pendingReward + ", Claimed: " + _rewardClaimed);

        if (rewardButton == null || buttonText == null)
        {
            Debug.LogError("[MoneyRewardButton] Button references are null!");
            return;
        }

        if (_rewardClaimed)
        {
            rewardButton.interactable = false;
            buttonText.text = claimedText;
            Debug.Log("[MoneyRewardButton] Button state: CLAIMED");
        }
        else
        {
            rewardButton.interactable = true;
            buttonText.text = string.Format(buttonFormat, _pendingReward);
            Debug.Log("[MoneyRewardButton] Button state: READY - " + _pendingReward + " gold");
        }
    }

    public void ResetForNewEncounter()
    {
        Debug.Log("[MoneyRewardButton] Resetting for new encounter");
        _rewardClaimed = false;
        _pendingReward = 0;
        UpdateButtonState();
    }

    private void OnDestroy()
    {
        if (rewardButton != null)
            rewardButton.onClick.RemoveListener(ClaimReward);
    }
}