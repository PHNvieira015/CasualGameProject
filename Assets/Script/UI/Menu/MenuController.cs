using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MenuController : MonoBehaviour
{
    public static MenuController Instance { get; private set; }

    public enum Screens
    {
        None,
        Main,
        Loading,
        CharacterSelect,
        Settings,
        CombatMenu,
        MapMenu,
        QuitConfirm,
        Reward,
        CardSelection,
        Defeat,
        DeckGallery,
        DiscardPile,
        DrawPile,
        RestSite,
        Shop,
        Victory
    }

    [Header("Screen References")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject characterSelect;
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject combatMenuScreen;
    [SerializeField] private GameObject mapScreen;
    [SerializeField] private GameObject quitConfirm;
    [SerializeField] private GameObject messageScreen;
    [SerializeField] private GameObject rewardScreen;
    [SerializeField] private GameObject defeatScreen;
    [SerializeField] private GameObject cardSelection;
    [SerializeField] private GameObject combatStartScreen;
    [SerializeField] private GameObject deckGalleryScreen;
    [SerializeField] private GameObject discardPileGalleryScreen;
    [SerializeField] private GameObject drawPileGalleryScreen;
    [SerializeField] private GameObject restSiteScreen; // Added rest site reference
    [SerializeField] private GameObject shopScreen; // Added shop reference
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private TMP_Text messageTXT;

    [Header("Configuration")]
    [SerializeField] private float screenTransitionDelay = 0.2f;

    private Screens currentScreen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void ChangeScreen(Screens screen)
    {
        if (currentScreen == screen) return;
        StartCoroutine(ChangeScreenWithDelay(screen));
    }

    public void SetScreenActive(Screens screen, bool active)
    {
        switch (screen)
        {
            case Screens.Main: mainMenu.SetActive(active); break;
            case Screens.Loading: loadingScreen.SetActive(active); break;
            case Screens.CharacterSelect: characterSelect.SetActive(active); break;
            case Screens.Settings: settings.SetActive(active); break;
            case Screens.QuitConfirm: quitConfirm.SetActive(active); break;
            case Screens.CombatMenu: combatMenuScreen.SetActive(active); break;
            case Screens.MapMenu: mapScreen.SetActive(active); break;
            case Screens.Reward: rewardScreen.SetActive(active); break;
            case Screens.Defeat: defeatScreen.SetActive(active); break;
            case Screens.CardSelection: cardSelection.SetActive(active); break;
            case Screens.DeckGallery: deckGalleryScreen.SetActive(active); break;
            case Screens.DiscardPile: discardPileGalleryScreen.SetActive(active); break;
            case Screens.DrawPile: drawPileGalleryScreen.SetActive(active); break;
            case Screens.RestSite:
                if (restSiteScreen != null) restSiteScreen.SetActive(active);
                break;
            case Screens.Shop:
                if (shopScreen != null) shopScreen.SetActive(active);
                break;
            case Screens.Victory:
                if (victoryScreen != null) victoryScreen.SetActive(active);
                break;
        }
    }

    private IEnumerator ChangeScreenWithDelay(Screens screen)
    {
        SetScreenActive(currentScreen, false);

        if (screenTransitionDelay > 0)
            yield return new WaitForSeconds(screenTransitionDelay);

        currentScreen = screen;
        SetScreenActive(screen, true);
    }

    public void ToggleCombatMenu()
    {
        bool shouldActivate = !combatMenuScreen.activeSelf;
        combatMenuScreen.SetActive(shouldActivate);
        currentScreen = shouldActivate ? Screens.CombatMenu : Screens.None;
    }

    public void ToggleMapScreen()
    {
        bool shouldActivate = !mapScreen.activeSelf;
        mapScreen.SetActive(shouldActivate);
        currentScreen = shouldActivate ? Screens.MapMenu : Screens.None;
    }

    public void ToggleShopScreen()
    {
        if (shopScreen == null)
        {
            Debug.LogWarning("Shop screen reference is not set!");
            return;
        }

        bool shouldActivate = !shopScreen.activeSelf;
        shopScreen.SetActive(shouldActivate);
        currentScreen = shouldActivate ? Screens.Shop : Screens.None;
    }

    public void ToggleRestSiteScreen()
    {
        if (restSiteScreen == null)
        {
            Debug.LogWarning("Rest site screen reference is not set!");
            return;
        }

        bool shouldActivate = !restSiteScreen.activeSelf;
        restSiteScreen.SetActive(shouldActivate);
        currentScreen = shouldActivate ? Screens.RestSite : Screens.None;
    }

    public void EndTurn()
    {
        combatMenuScreen.SetActive(false);
        Debug.Log("Turn ended");
    }

    public void ShowMessage(string message, float duration = 3f)
    {
        messageTXT.text = message;
        messageScreen.SetActive(true);

        if (duration > 0)
            Invoke(nameof(HideMessage), duration);
    }

    private void HideMessage()
    {
        messageScreen.SetActive(false);
    }

    public void ShowVictoryScreen()
    {
        ChangeScreen(Screens.Victory);
        Debug.Log("Victory screen shown!");
    }

    public void HideVictoryScreen()
    {
        if (currentScreen == Screens.Victory)
        {
            ChangeScreen(Screens.None);
        }
    }

    public void ShowShopScreen()
    {
        ChangeScreen(Screens.Shop);
        Debug.Log("Shop screen shown!");
    }

    public void HideShopScreen()
    {
        if (currentScreen == Screens.Shop)
        {
            ChangeScreen(Screens.None);
        }
    }

    public void ShowRestSiteScreen()
    {
        ChangeScreen(Screens.RestSite);
        Debug.Log("Rest site screen shown!");
    }

    public void HideRestSiteScreen()
    {
        if (currentScreen == Screens.RestSite)
        {
            ChangeScreen(Screens.None);
        }
    }

    public void StartGame(int sceneId)
    {
        ChangeScreen(Screens.Loading);
        SceneManager.LoadScene(sceneId);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting application...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // Helper method to check if a specific screen is active
    public bool IsScreenActive(Screens screen)
    {
        return currentScreen == screen;
    }

    // Method to close all screens
    public void CloseAllScreens()
    {
        SetScreenActive(currentScreen, false);
        currentScreen = Screens.None;
    }
}