// In your Reward System
using UnityEngine;

public class MoneyRewardSystem : MonoBehaviour
{
    public void GiveBattleReward(int moneyReward, int keyReward)
    {
        MoneyKeySystem.Instance.AddMoney(moneyReward);
        MoneyKeySystem.Instance.AddKeys(keyReward);

        Debug.Log($"Reward given: {moneyReward} money, {keyReward} keys");
    }
}