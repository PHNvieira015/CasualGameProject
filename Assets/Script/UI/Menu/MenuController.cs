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
        Shop
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
}