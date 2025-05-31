using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuController2 : MonoBehaviour
{
    /*
    public static MenuController instance;

    [System.Serializable]
    public enum Screens
    {
        None,
        Main,
        Loading,
        Login,
        options,
        video,
        audio,
        controls,
        credit,
        CreateAccount,
        RecoverAccount,
        CreateRoom,
        Ranking,
        lobby,
        Shop
    }
    public Screens currentScreen;

    [Header("Screens")]
    [SerializeField] GameObject MainScreen;
    [SerializeField] GameObject loadingScreen;
    [SerializeField] GameObject loginScreen;
    [SerializeField] GameObject creditScreen;
    [SerializeField] GameObject optionsScreen;
    [SerializeField] GameObject videoScreen;
    [SerializeField] GameObject audioScreen;
    [SerializeField] GameObject controlsScreen;
    [SerializeField] GameObject createAccountScreen;
    [SerializeField] GameObject messageScreen;
    [SerializeField] GameObject recoverAccountScreen;
    [SerializeField] GameObject rankingScreen;
    [SerializeField] GameObject lobbyScreen;
    [SerializeField] GameObject ShopScreen;
    [SerializeField] GameObject createRoomScreen;
    [SerializeField] TextMeshProUGUI messageTXT;
    public Image LoadingBarFill;

    [Header("Login Information")]
    [SerializeField] TMP_InputField inputUsernameOrEmailLogin;
    [SerializeField] TMP_InputField inputPasswordLogin;
    [SerializeField] TextMeshProUGUI nickName;

    [Header("Create Account Information")]
    public TMP_InputField inputUsernameCreateAccount;
    [SerializeField] TMP_InputField inputEmailCreateAccount;
    [SerializeField] TMP_InputField inputPasswordCreateAccount;
    [SerializeField] TMP_InputField inputConfirmPasswordCreateAccount;

    [Header("Recover Account")]
    [SerializeField] TMP_InputField inputRecoverAccount;

    [Header("Rankings")]
    [SerializeField] string rankingName;
    [SerializeField] TMP_InputField rankingValue;
    [SerializeField] TextMeshProUGUI rankingTxt;
    [SerializeField] TextMeshProUGUI rankingListTxt;

    [Header("Shop")]
    [SerializeField] TextMeshProUGUI coinsTXT;

    [Header("Hover UI")]
    [SerializeField] private Image hoverImageDisplay; // Image to display on hover
    [SerializeField] private Sprite[] hoverSprites; // Array of sprites for each icon

    public Animator Animator;
    [HideInInspector] public Screens TargetScreen;

    private void Awake()
    {
        instance = this;
        ShowScreen(Screens.Main);
        messageScreen.SetActive(false);
        if (hoverImageDisplay != null)
        {
            hoverImageDisplay.gameObject.SetActive(false); // Ensure the hover image is initially hidden
        }
    }

    public void ShowScreen(Screens _screen)
    {
        Debug.Log("show Screen");
        currentScreen = _screen;
        MainScreen.SetActive(false);
        loadingScreen.SetActive(false);
        creditScreen.SetActive(false);
        creditScreen.SetActive(false);
        loginScreen.SetActive(false);
        videoScreen.SetActive(false);
        audioScreen.SetActive(false);
        optionsScreen.SetActive(false);
        controlsScreen.SetActive(false);
        createAccountScreen.SetActive(false);
        recoverAccountScreen.SetActive(false);
        ShopScreen.SetActive(false);
        rankingScreen.SetActive(false);
        lobbyScreen.SetActive(false);
        createRoomScreen.SetActive(false);

        switch (currentScreen)
        {
            case Screens.None:
                break;
            case Screens.Main:
                MainScreen.SetActive(true);
                break;
            case Screens.Loading:
                loadingScreen.SetActive(true);
                break;
            case Screens.Login:
                loginScreen.SetActive(true);
                break;
               case Screens.credit:
                creditScreen.SetActive(true);
                break;
            case Screens.options:
                optionsScreen.SetActive(true);
                break;
            case Screens.video:
                videoScreen.SetActive(true);
                break;
            case Screens.audio:
                audioScreen.SetActive(true);
                break;
            case Screens.controls:
                controlsScreen.SetActive(true);
                break;
            case Screens.CreateAccount:
                createAccountScreen.SetActive(true);
                break;
            case Screens.RecoverAccount:
                recoverAccountScreen.SetActive(true);
                break;
            case Screens.Ranking:
                StartCoroutine(ShowRanking());
                break;
            case Screens.Shop:
                ShopScreen.SetActive(true);
                break;
            case Screens.lobby:
                lobbyScreen.SetActive(true);
                break;
            case Screens.CreateRoom:
                createRoomScreen.SetActive(true);
                break;
        }
    }

    IEnumerator ShowRanking()
    {
        Debug.Log("show ranking");
        PlayfabManager.instance.apiFinished = false;
        PlayfabManager.instance.GetLeaderboard();


        yield return new WaitUntil(() => PlayfabManager.instance.apiFinished);
        rankingScreen.SetActive(true);
        
    }




    public void ShowScreen(int screen)
    {
        ShowScreen((Screens)screen);
    }

    public void GoToTransition()
    {
        ShowScreen(TargetScreen);
    }

    #region Login
    public void BtnLogin()
    {
        ShowScreen(Screens.Loading);

        string _usernameOrEmail = inputUsernameOrEmailLogin.text;
        string _password = inputPasswordLogin.text;
        nickName.text = inputUsernameOrEmailLogin.text;

        if (string.IsNullOrEmpty(_usernameOrEmail) || string.IsNullOrEmpty(_password))
        {
            ShowMessage("Favor preencher todos os campos!");
            ShowScreen(Screens.Login);
        }
        else if (_usernameOrEmail.Length < 3)
        {
            ShowMessage("Dados de usuário inválidos!");
            ShowScreen(Screens.Login);
        }
        else
        {
            PlayfabManager.instance.UserLogin(_usernameOrEmail, _password);
        }
    }

    public void BtnCreateAccount()
    {
        ShowScreen(Screens.Loading);

        string _username = inputUsernameCreateAccount.text;
        string _email = inputEmailCreateAccount.text;
        string _password = inputPasswordCreateAccount.text;
        string _confirmPassword = inputConfirmPasswordCreateAccount.text;

        if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_email) || string.IsNullOrEmpty(_password) || string.IsNullOrEmpty(_confirmPassword))
        {

            ShowMessage("Favor preencher todos os campos!");
            ShowScreen(Screens.CreateAccount);
        }
        else if (_username.Length < 3)
        {
            ShowMessage("Username precisa ter ao menos 3 caracteres");
            ShowScreen(Screens.CreateAccount);
        }
        else if (_password != _confirmPassword)
        {
            ShowMessage("A senha não confere! Favor verificar a senha digitada!");
            ShowScreen(Screens.CreateAccount);
        }
        else
        {
            PlayfabManager.instance.CreateAccount(_username, _email, _password);
        }
    }

    public void BtnBackToLogin()
    {
        ShowScreen(Screens.Login);
    }
    public void BtnGoToCreateAccount()
    {
        ShowScreen(Screens.CreateAccount);
    }

    public void BtnRecoverAccount()
    {
        PlayfabManager.instance.Recoverpassword(inputRecoverAccount.text);
    }
    public void BtnShowRecoverAccountScreen()
    {
        ShowScreen(Screens.RecoverAccount);
    }
    #endregion

    #region Others
    public void ShowMessage(string _message)
    {
        messageTXT.text = _message;
        messageScreen.SetActive(true);
    }
    public void BtnDeleteAccount()
    {
        PlayfabManager.instance.DeleteAccount();
    }
    #endregion

    public void UpdateRanking()
    {
        if (string.IsNullOrEmpty(rankingValue.text))
        {
            ShowMessage("Favor informar o valor a ser atualizado!");
            return;
        }
        ShowScreen(Screens.Loading);
        PlayfabManager.instance.UpdatePlayerScore(rankingName, int.Parse(rankingValue.text));
    }

    public void UpdatePlayerRanking(int _value)
    {
        rankingTxt.text = "Ranking Atual: " + _value.ToString();
    }

    public void UpdateRankingLists(string _value)
    {
        rankingListTxt.text = _value;
    }

    public void StartGame(int sceneId)
    {
        ShowScreen(Screens.Loading);
        PlayfabManager.instance.GetPlayerInventory();
        StartCoroutine(LoadSceneAsync(sceneId));
        Debug.Log("Game Started");
    }

    public void LoadScene(int sceneId)
    {
        StartCoroutine(LoadSceneAsync(sceneId));
    }

    IEnumerator LoadSceneAsync(int sceneId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);
        ShowScreen(Screens.Loading);

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            //LoadingBarFill.fillAmount = progressValue;
            yield return null;
        }
    }

    public void ExitButton()
    {
        Application.Quit();
        Debug.Log("Game Closed");
    }

    public void BuyTwoHandedAxe()
    {
        PlayfabManager.instance.BuyItemToPlayer("WP001001", "GC", 0);
    }

    public void BuyBow()
    {
        PlayfabManager.instance.BuyItemToPlayer("WP001002", "GC", 100);
    }

    public void BuyItem(int _price)
    {
        PlayfabManager.instance.BuyItemToPlayer("WP001002", "GC", 0);
    }

    public void UpdateCoins(int amount)
    {
        coinsTXT.text = amount.ToString();
    }

    // Function to show the hover image
    public void ShowHoverImage(Sprite sprite)
    {
        if (hoverImageDisplay != null)
        {
            hoverImageDisplay.sprite = sprite;
            hoverImageDisplay.gameObject.SetActive(true);
        }
    }

    // Function to hide the hover image
    public void HideHoverImage()
    {
        if (hoverImageDisplay != null)
        {
            hoverImageDisplay.gameObject.SetActive(false);
        }
    }

    // Method to be called when an icon is hovered over
    public void OnIconHover(int index)
    {
        if (index >= 0 && index < hoverSprites.Length)
        {
            ShowHoverImage(hoverSprites[index]);
        }
    }

    // Method to be called when an icon is no longer hovered over
    public void OnIconExit()
    {
        HideHoverImage();
    }
    */

}
