using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ScreenButton : MonoBehaviour
{
    [SerializeField] private MenuController.Screens targetScreen;
    [SerializeField] private bool useTransition = true;
    [SerializeField] private bool isCombatMenuButton = false;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        if (isCombatMenuButton)
        {
            MenuController.Instance.ToggleCombatMenu();
        }
        else if (useTransition)
        {
            MenuController.Instance.ChangeScreen(targetScreen);
        }
        else
        {
            MenuController.Instance.SetScreenActive(targetScreen, true);
        }
    }
}