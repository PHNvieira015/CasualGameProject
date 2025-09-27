using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DefeatScreenUIHandler : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button retryButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        // Setup button listeners
        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetryClicked);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnRetryClicked()
    {
        Debug.Log("Retry button clicked!");

        if (MenuController.Instance != null)
        {
            MenuController.Instance.RetryGame();
        }
        else
        {
            // Fallback
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void OnMainMenuClicked()
    {
        Debug.Log("Main menu button clicked from defeat screen!");

        if (MenuController.Instance != null)
        {
            MenuController.Instance.ReturnToMainMenu();
        }
        else
        {
            // Fallback
            SceneManager.LoadScene(0);
        }
    }

    private void OnQuitClicked()
    {
        Debug.Log("Quit button clicked from defeat screen!");

        if (MenuController.Instance != null)
        {
            MenuController.Instance.QuitGame();
        }
        else
        {
            // Fallback
            Application.Quit();
        }
    }
}