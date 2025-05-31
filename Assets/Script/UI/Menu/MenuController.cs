using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    public enum Screens
    {
        None,
        Main,
        Loading,
        CharacterSelect,
        Settings,
        QuitConfirm
    }
    public Screens currentScreen;

    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject characterSelect;   
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject quitConfirm;
    [SerializeField] private GameObject messageScreen;
    [SerializeField] private TMP_Text messageTXT;

    [HideInInspector] public Screens TargetScreen;

    // Method to show or hide screens based on the current screen
    public void ShowScreen(Screens _screen)
    {
        Debug.Log("Showing screen: " + _screen.ToString());

        // Deactivate all screens first
        MainMenu.SetActive(false);
        loadingScreen.SetActive(false);
        characterSelect.SetActive(false);
        settings.SetActive(false);
        quitConfirm.SetActive(false);
        messageScreen.SetActive(false);

        // Activate the desired screen based on the passed screen
        switch (_screen)
        {
            case Screens.None:
                MainMenu.SetActive(true);
                Debug.Log("Main screen activated");
                break;
            case Screens.Main:
                MainMenu.SetActive(true);
                Debug.Log("Main screen activated");
                break;
            case Screens.Loading:
                loadingScreen.SetActive(true);
                break;
            case Screens.CharacterSelect:
                characterSelect.SetActive(true);
                break;
            case Screens.Settings:
                settings.SetActive(true);
                break;
            case Screens.QuitConfirm:
                quitConfirm.SetActive(true);
                break;
        }

        currentScreen = _screen;
    }

    // Overloaded method to handle screen index
    public void ShowScreen(int screen)
    {
        ShowScreen((Screens)screen);
    }

    public void ShowMessage(string _message)
    {
        messageTXT.text = _message;
        messageScreen.SetActive(true);
    }

    public void StartGame(int sceneId)
    {
        // Load the scene (change sceneId to load the desired scene)
        ShowScreen(Screens.Loading); // Show loading screen during scene load
        SceneManager.LoadScene(sceneId);
        Debug.Log("Game Started");
    }

    public void ExitButton()
    {
        Application.Quit();
        Debug.Log("Game Closed");
    }
 }