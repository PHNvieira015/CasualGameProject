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
        QuitConfirm,
        Reward,
        CardSelection,
        Defeat,
        CombatStart,
        DeckGallery,
        DiscardPile,
        DrawPile
    }
    public Screens currentScreen;

    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject characterSelect;
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject quitConfirm;
    [SerializeField] private GameObject messageScreen;
    [SerializeField] private GameObject RewardScreen;
    [SerializeField] private GameObject CardSelection;
    [SerializeField] private GameObject DefeatScreen;
    [SerializeField] private GameObject CombatStartScreen;
    [SerializeField] private GameObject DeckGalleryScreen;
    [SerializeField] private GameObject DiscardPileGalleryScreen;
    [SerializeField] private GameObject DrawPileGalleryScreen;
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
        RewardScreen.SetActive(false);
        DefeatScreen.SetActive(false);
        CombatStartScreen.SetActive(false);
        DeckGalleryScreen.SetActive(false);
        DiscardPileGalleryScreen.SetActive(false);
        DrawPileGalleryScreen.SetActive(false);

        // Activate the desired screen based on the passed screen
        switch (_screen)
        {
            case Screens.None:
                // No screens active
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
            case Screens.Reward:
                RewardScreen.SetActive(true);
                break;
            case Screens.CardSelection:
                CardSelection.SetActive(true);
                break;
            case Screens.Defeat:
                DefeatScreen.SetActive(true);
                break;
            case Screens.CombatStart:
                CombatStartScreen.SetActive(true);
                break;
            case Screens.DeckGallery:
                DeckGalleryScreen.SetActive(true);
                break;
            case Screens.DiscardPile:
                DiscardPileGalleryScreen.SetActive(true);
                break;
            case Screens.DrawPile:
                DrawPileGalleryScreen.SetActive(true);
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