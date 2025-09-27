using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class VictoryScreenManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject victoryScreen;
    public TextMeshProUGUI victoryTitleText;
    public TextMeshProUGUI bossNameText;
    public Button mainMenuButton;
    public Button quitButton;

    private void Start()
    {
        // Setup button listeners
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        // Hide victory screen on start
        if (victoryScreen != null)
            victoryScreen.SetActive(false);
    }

    public void ShowVictoryScreen(string bossName = "The Final Boss")
    {
        if (victoryScreen != null)
        {
            // Update UI texts
            if (victoryTitleText != null)
                victoryTitleText.text = "VICTORY!";

            if (bossNameText != null)
                bossNameText.text = $"You defeated {bossName}!";

            victoryScreen.SetActive(true);
            Debug.Log($"Victory screen shown for defeating {bossName}!");
        }
    }

    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}