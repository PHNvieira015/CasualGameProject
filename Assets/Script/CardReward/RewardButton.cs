using UnityEngine;
using UnityEngine.UI;

public class RewardButton : MonoBehaviour
{
    [SerializeField] private Button rewardButton;
    [SerializeField] private CardRewardSystem rewardSystem;

    void Start()
    {
        rewardButton.onClick.AddListener(ShowRewards);
    }

    void ShowRewards()
    {
        rewardSystem.ShowRewards();
        rewardButton.interactable = false;
    }

    // Call this via UnityEvent when rewards are closed
    public void OnRewardsClosed()
    {
        rewardButton.interactable = true;
    }
}