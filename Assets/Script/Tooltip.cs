using UnityEngine;

public class Tooltip : MonoBehaviour
{
    public string message;

    public void OnMouseEnter()
    {
        TooltipManager.Instance.SetAndShowTooltip(message);
    }

    private void OnMouseExit()
    {
        TooltipManager.Instance.HideTooltip();
    }
}