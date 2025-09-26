using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; private set; }

    public TextMeshProUGUI textComponent;
    public RectTransform backgroundRectTransform;
    public Vector2 padding = new Vector2(8, 4);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Optional: Uncomment if you want this to persist across scenes
        // DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Ensure tooltip is hidden at start
        gameObject.SetActive(false);

        // Auto-find text component if not assigned
        if (textComponent == null)
        {
            textComponent = GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            // Follow mouse position with offset to avoid covering cursor
            Vector2 mousePosition = Input.mousePosition;
            Vector2 tooltipPosition = new Vector2(mousePosition.x + 15, mousePosition.y - 15);

            // Keep tooltip on screen
            tooltipPosition = KeepTooltipOnScreen(tooltipPosition);

            transform.position = tooltipPosition;
        }
    }

    public void SetAndShowTooltip(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            HideTooltip();
            return;
        }

        if (textComponent != null)
        {
            textComponent.text = message;
            gameObject.SetActive(true);

            // Optional: Adjust background size to fit text
            AdjustBackgroundSize();
        }
        else
        {
            Debug.LogWarning("TextComponent is not assigned in TooltipManager");
        }
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);

        if (textComponent != null)
        {
            textComponent.text = string.Empty;
        }
    }

    private void AdjustBackgroundSize()
    {
        if (backgroundRectTransform != null && textComponent != null)
        {
            // Calculate the preferred size of the text
            Vector2 textSize = new Vector2(
                textComponent.preferredWidth,
                textComponent.preferredHeight
            );

            // Set background size with padding
            backgroundRectTransform.sizeDelta = textSize + padding;
        }
    }

    private Vector2 KeepTooltipOnScreen(Vector2 desiredPosition)
    {
        if (backgroundRectTransform == null)
            return desiredPosition;

        Vector2 tooltipSize = backgroundRectTransform.sizeDelta;
        Vector2 screenBounds = new Vector2(Screen.width, Screen.height);

        // Check right edge
        if (desiredPosition.x + tooltipSize.x > screenBounds.x)
        {
            desiredPosition.x = screenBounds.x - tooltipSize.x - 5;
        }

        // Check left edge
        if (desiredPosition.x < 0)
        {
            desiredPosition.x = 5;
        }

        // Check top edge
        if (desiredPosition.y > screenBounds.y)
        {
            desiredPosition.y = screenBounds.y - 5;
        }

        // Check bottom edge
        if (desiredPosition.y - tooltipSize.y < 0)
        {
            desiredPosition.y = tooltipSize.y + 5;
        }

        return desiredPosition;
    }

    // Optional: Static methods for easier access
    public static void ShowTooltip(string message)
    {
        if (Instance != null)
        {
            Instance.SetAndShowTooltip(message);
        }
        else
        {
            Debug.LogWarning("TooltipManager instance not found!");
        }
    }

    public static void HideTooltipStatic()
    {
        if (Instance != null)
        {
            Instance.HideTooltip();
        }
    }
}