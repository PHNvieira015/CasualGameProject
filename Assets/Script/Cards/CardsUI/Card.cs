using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    #region Properties/Fields

    public RectTransform Rect { get { return _rect; } }

    RectTransform _rect;
    Transform _back;
    Transform _front;

    int _movementTween;
    int _rotationTween;

    #endregion

    #region Unity Events
    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _back = transform.Find("Back");
        _front = transform.Find("Front");
    }
    private void Update()
    {
        if (_rect.rotation.eulerAngles.y > 90 && _rect.rotation.eulerAngles.y < 270)
        {
            _back.SetAsLastSibling();
        }
        else
        {
            _front.SetAsLastSibling();
        }
    }
    #endregion

    #region Methods
    public void Move(Vector3 position, float duration, System.Action onComplete)
    {
        if (LeanTween.isTweening(this.gameObject) && LeanTween.isTweening(_movementTween))
        {
            LeanTween.cancel(_movementTween);
        }

        _movementTween = _rect.LeanMove(position, duration).setOnComplete(onComplete).id;

    }
    public void Rotate(float amount, float duration)
    {
        // Comprehensive null checking
        if (this == null)
        {
            Debug.LogError("Card.Rotate: 'this' is null!");
            return;
        }

        if (gameObject == null)
        {
            Debug.LogError("Card.Rotate: gameObject is null!");
            return;
        }

        if (!gameObject.activeInHierarchy)
        {
            Debug.LogError($"Card.Rotate: GameObject {gameObject.name} is not active!");
            return;
        }

        // Ensure RectTransform is available
        if (_rect == null)
        {
            Debug.LogWarning("Card.Rotate: _rect is null, trying to get component...");
            _rect = GetComponent<RectTransform>();

            if (_rect == null)
            {
                Debug.LogError("Card.Rotate: RectTransform component not found!");
                return;
            }
        }

        // Additional check - is the RectTransform valid?
        if (_rect == null)
        {
            Debug.LogError("Card.Rotate: RectTransform is still null after initialization!");
            return;
        }

        // Cancel existing tween if any
        if (LeanTween.isTweening(gameObject) && LeanTween.isTweening(_rotationTween))
        {
            LeanTween.cancel(_rotationTween);
        }

        Debug.Log($"Card.Rotate: Rotating card {gameObject.name} by {amount} degrees");
        _rotationTween = LeanTween.rotateAroundLocal(_rect, Vector3.up, amount, duration).id;
    }

    public bool CanPlay()
    {
        // Add comprehensive null and state checks
        if (StateMachine.Instance == null)
            return false;

        if (StateMachine.Instance.Current == null)
            return false;

        if (StateMachine.Instance.CurrentUnit == null)
            return false;

        // Check if we're in the correct state for playing cards
        if (StateMachine.Instance.Current.GetType() != typeof(PlayCardsState))
            return false;

        // Check if the current unit is a player unit
        if (StateMachine.Instance.CurrentUnit.GetType() != typeof(PlayerUnit))
            return false;

        // Check playability conditions
        foreach (IPlayability playability in GetComponents<IPlayability>())
        {
            if (!playability.CanPlay())
            {
                return false;
            }
        }

        return true;
    }

    #endregion
}