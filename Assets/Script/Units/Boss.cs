using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("Boss Settings")]
    [SerializeField] private string bossName = "Final Boss";

    private bool isDefeated = false;

    public string BossName => bossName;

    // This method should be called when the boss dies
    public void HandleBossDeath()
    {
        if (isDefeated) return;

        isDefeated = true;
        Debug.Log($"BOSS DEFEATED: {bossName}!");

        ShowVictoryScreen();
    }

    private void ShowVictoryScreen()
    {
        // Try to find VictoryScreenManager first
        GameObject victoryManagerObj = GameObject.Find("VictoryScreenManager");
        if (victoryManagerObj != null)
        {
            VictoryScreenManager victoryManager = victoryManagerObj.GetComponent<VictoryScreenManager>();
            if (victoryManager != null)
            {
                victoryManager.ShowVictoryScreen(bossName);
                return;
            }
        }

        // Fallback to MenuController
        if (MenuController.Instance != null)
        {
            MenuController.Instance.ChangeScreen(MenuController.Screens.Victory);
            return;
        }

        // Ultimate fallback
        Debug.Log($"VICTORY! {bossName} defeated!");
    }

    [ContextMenu("Test Boss Defeat")]
    public void TestBossDefeat()
    {
        HandleBossDeath();
    }
}