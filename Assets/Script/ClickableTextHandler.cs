using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ClickableTextHandler : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI _tmpText;

    void Awake()
    {
        _tmpText = GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(_tmpText, eventData.position, eventData.pressEventCamera);
        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = _tmpText.textInfo.linkInfo[linkIndex];
            string linkID = linkInfo.GetLinkID();

            Debug.Log($"Clicked on link with ID: {linkID}");

            // Example: Trigger different actions based on the linkID
            if (linkID == "MyClickableWord")
            {
                Debug.Log("Performed action for 'MyClickableWord'");
                // Call a specific method or raise a custom event
            }
            else if (linkID == "AnotherLink")
            {
                Debug.Log("Performed action for 'AnotherLink'");
            }
        }
    }
}