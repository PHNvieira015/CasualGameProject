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
        CalculatePendingReward();
    }

    public void CalculatePendingReward()
    {
        if (EncounterManager.Instance == null)
        {
            Debug.LogError("[MoneyRewardButton] EncounterManager not found!");
            _pendingReward = 0;
            UpdateButtonState();
            return;
        }

        _pendingReward = EncounterManager.Instance.CalculateBattleReward();
        _rewardClaimed = false;
        UpdateButtonState();
    }

    public void ClaimReward()
    {
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

        if (MoneyKeySystem.Instance.AddMoney(_pendingReward))
        {
            _rewardClaimed = true;
            UpdateButtonState();
        }
        else
        {
            Debug.LogError("[MoneyRewardButton] Failed to add money reward!");
        }
    }

    private void UpdateButtonState()
    {
        if (rewardButton == null || buttonText == null)
        {
            Debug.LogError("[MoneyRewardButton] Button references are null!");
            return;
        }

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
}