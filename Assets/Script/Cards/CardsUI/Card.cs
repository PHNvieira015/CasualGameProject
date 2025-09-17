using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public RectTransform Rect { get { return rect; } }

    private RectTransform rect;
    private Transform back;
    private Transform front;

    private int movementTween;
    private int rotationTween;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        back = transform.Find("Back");
        front = transform.Find("Front");
    }

    private void Update()
    {
        if (rect != null && rect.rotation.eulerAngles.y > 90 && rect.rotation.eulerAngles.y < 270)
        {
            if (back != null) back.SetAsLastSibling();
        }
        else
        {
            if (front != null) front.SetAsLastSibling();
        }
    }

    public void Move(Vector3 position, float duration, System.Action onComplete)
    {
        if (rect == null)
        {
            Debug.LogError("Card.Move: RectTransform is null");
            onComplete?.Invoke();
            return;
        }

        if (LeanTween.isTweening(gameObject) && LeanTween.isTweening(movementTween))
        {
            LeanTween.cancel(movementTween);
        }

        movementTween = LeanTween.move(rect, position, duration)
            .setOnComplete(onComplete)
            .id;
    }

    public void Rotate(float amount, float duration)
    {
        if (this == null)
        {
            Debug.LogError("Card.Rotate: this is null");
            return;
        }

        if (gameObject == null)
        {
            Debug.LogError("Card.Rotate: gameObject is null");
            return;
        }

        if (!gameObject.activeInHierarchy)
        {
            Debug.LogError("Card.Rotate: GameObject " + gameObject.name + " is not active");
            return;
        }

        if (rect == null)
        {
            Debug.LogWarning("Card.Rotate: rect is null, trying to get component");
            rect = GetComponent<RectTransform>();

            if (rect == null)
            {
                Debug.LogError("Card.Rotate: RectTransform component not found");
                return;
            }
        }

        if (LeanTween.isTweening(gameObject) && LeanTween.isTweening(rotationTween))
        {
            LeanTween.cancel(rotationTween);
        }

        Debug.Log("Card.Rotate: Rotating card " + gameObject.name + " by " + amount + " degrees");
        rotationTween = LeanTween.rotateAroundLocal(rect, Vector3.up, amount, duration).id;
    }

    public bool CanPlay()
    {
        if (StateMachine.Instance == null)
            return false;

        if (StateMachine.Instance.Current == null)
            return false;

        if (StateMachine.Instance.CurrentUnit == null)
            return false;

        if (StateMachine.Instance.Current.GetType() != typeof(PlayCardsState))
            return false;

        if (StateMachine.Instance.CurrentUnit.GetType() != typeof(PlayerUnit))
            return false;

        foreach (IPlayability playability in GetComponents<IPlayability>())
        {
            if (!playability.CanPlay())
            {
                return false;
            }
        }

        return true;
    }
}