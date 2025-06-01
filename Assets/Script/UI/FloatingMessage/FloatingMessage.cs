using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class FloatingMessage : MonoBehaviour, IInGameMessage
{
    [Header("Positioning")]
    [Tooltip("Local offset from parent object's position")]
    public Vector2 StartOffset = new Vector2(0f, 1f);

    [Header("Physics")]
    public float InitialYVelocity = 7f;
    public float InitialXVelocityRange = 3f;

    [Header("Timing")]
    public float LifeTime = 0.8f;
    public float FadeStartTime = 0.5f;

    private Rigidbody2D _rigidbody;
    private TMP_Text _textComponent;
    private Color _originalColor;
    private float _elapsedTime;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _textComponent = GetComponentInChildren<TMP_Text>();
        if (_textComponent != null) _originalColor = _textComponent.color;
    }

    private void Start()
    {
        // Set starting position relative to parent
        if (transform.parent != null)
        {
            transform.position = transform.parent.position + (Vector3)StartOffset;
        }

        // Apply physics
        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = new Vector2(
                Random.Range(-InitialXVelocityRange, InitialXVelocityRange),
                InitialYVelocity
            );
        }

        Destroy(gameObject, LifeTime);
    }

    public void SetMessage(string msg)
    {
        if (_textComponent != null) _textComponent.text = msg;
    }

    public void SetColor(Color color)
    {
        if (_textComponent != null)
        {
            _textComponent.color = color;
            _originalColor = color;
        }
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime >= FadeStartTime && _textComponent != null)
        {
            float fadeProgress = (_elapsedTime - FadeStartTime) / (LifeTime - FadeStartTime);
            _textComponent.color = Color.Lerp(_originalColor,
                new Color(_originalColor.r, _originalColor.g, _originalColor.b, 0),
                fadeProgress);
        }
    }
}