using UnityEngine;
using UnityEngine.EventSystems;

public class HoverText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject textObject; // Drag your text object here

    public void OnPointerEnter(PointerEventData eventData)
    {
        textObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        textObject.SetActive(false);
    }
}