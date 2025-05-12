using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class SelectableUnit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Color HighlightedColor;
    SpriteRenderer _spriteRenderer;
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        _spriteRenderer.color = HighlightedColor;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        _spriteRenderer.color = Color.white;
    }

}
