using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CardTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private float showDelay = 0.3f;

    private bool _isHovering;
    private Coroutine _showCoroutine;

    void Awake()
    {
        // Hide tooltip on start
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHovering = true;
        _showCoroutine = StartCoroutine(ShowAfterDelay());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovering = false;
        HideTooltip();

        if (_showCoroutine != null)
        {
            StopCoroutine(_showCoroutine);
            _showCoroutine = null;
        }
    }

    private IEnumerator ShowAfterDelay()
    {
        yield return new WaitForSeconds(showDelay);

        if (_isHovering && tooltipPanel != null)
        {
            tooltipPanel.SetActive(true);
        }
    }

    private void HideTooltip()
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }

    void OnDisable()
    {
        HideTooltip();
        _isHovering = false;
    }
}