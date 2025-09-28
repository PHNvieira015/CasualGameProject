using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class CardTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Objects to Hide")]
    [SerializeField] private GameObject backObject;
    [SerializeField] private GameObject frontObject;

    [Header("Objects to Show")]
    [SerializeField] private GameObject cardHighlight;
    [SerializeField] private float showDelay = 0.3f;

    [Header("Source Components")]
    [SerializeField] private TextMeshProUGUI source_CardName_Text;
    [SerializeField] private UnityEngine.UI.Image source_Art;
    [SerializeField] private TextMeshProUGUI source_Description;
    [SerializeField] private TextMeshProUGUI source_EnergyCost_Text;

    [Header("Highlight Components")]
    [SerializeField] private TextMeshProUGUI highlight_CardName_Text;
    [SerializeField] private UnityEngine.UI.Image highlight_Art;
    [SerializeField] private TextMeshProUGUI highlight_Description;
    [SerializeField] private TextMeshProUGUI highlight_EnergyCost_Text;

    private bool _isHovering;
    private Coroutine _showCoroutine;

    void Awake()
    {
        // Ensure initial state is correct
        ResetToDefaultState();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHovering = true;
        _showCoroutine = StartCoroutine(ShowAfterDelay());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovering = false;
        ResetToDefaultState();

        if (_showCoroutine != null)
        {
            StopCoroutine(_showCoroutine);
            _showCoroutine = null;
        }
    }

    private IEnumerator ShowAfterDelay()
    {
        yield return new WaitForSeconds(showDelay);

        if (_isHovering)
        {
            ShowTooltipState();
        }
    }

    private void ShowTooltipState()
    {
        // Hide back object
        if (backObject != null)
            backObject.SetActive(false);

        // Hide all child objects of front object (but keep front object active)
        if (frontObject != null)
        {
            foreach (Transform child in frontObject.transform)
            {
                child.gameObject.SetActive(false);
            }
        }

        // Show highlight
        if (cardHighlight != null)
            cardHighlight.SetActive(true);

        // Copy all components from source to highlight
        CopyComponentsToHighlight();
    }

    private void CopyComponentsToHighlight()
    {
        // Copy CardName_Text
        if (source_CardName_Text != null && highlight_CardName_Text != null)
        {
            highlight_CardName_Text.text = source_CardName_Text.text;
        }

        // Copy Art
        if (source_Art != null && highlight_Art != null)
        {
            highlight_Art.sprite = source_Art.sprite;
            highlight_Art.color = source_Art.color;
        }

        // Copy Description
        if (source_Description != null && highlight_Description != null)
        {
            highlight_Description.text = source_Description.text;
        }

        // Copy EnergyCost_Text
        if (source_EnergyCost_Text != null && highlight_EnergyCost_Text != null)
        {
            highlight_EnergyCost_Text.text = source_EnergyCost_Text.text;
        }
    }

    private void ResetToDefaultState()
    {
        // Show back object
        if (backObject != null)
            backObject.SetActive(true);

        // Show all child objects of front object
        if (frontObject != null)
        {
            foreach (Transform child in frontObject.transform)
            {
                child.gameObject.SetActive(true);
            }
        }

        // Hide highlight
        if (cardHighlight != null)
            cardHighlight.SetActive(false);
    }

    void OnDisable()
    {
        ResetToDefaultState();
        _isHovering = false;

        if (_showCoroutine != null)
        {
            StopCoroutine(_showCoroutine);
            _showCoroutine = null;
        }
    }

    // Optional: Public method to update source components if card data changes during runtime
    public void UpdateCardData(string cardName, Sprite art, string description, string energyCost)
    {
        if (source_CardName_Text != null)
            source_CardName_Text.text = cardName;

        if (source_Art != null)
            source_Art.sprite = art;

        if (source_Description != null)
            source_Description.text = description;

        if (source_EnergyCost_Text != null)
            source_EnergyCost_Text.text = energyCost;
    }
}