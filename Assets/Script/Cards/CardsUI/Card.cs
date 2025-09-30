using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public RectTransform Rect { get { return rect; } }

    private RectTransform rect;
    private Transform back;
    private Transform front;

    private int movementTween = -1;
    private int rotationTween = -1;
    private bool isMoving = false;
    private System.Action onMoveComplete;

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

        // Cancel any existing movement
        if (movementTween != -1 && LeanTween.isTweening(movementTween))
        {
            LeanTween.cancel(movementTween);
        }

        isMoving = true;
        onMoveComplete = onComplete;

        movementTween = LeanTween.move(rect, position, duration)
            .setOnComplete(OnMoveComplete)
            .id;
    }

    private void OnMoveComplete()
    {
        isMoving = false;
        movementTween = -1;
        onMoveComplete?.Invoke();
        onMoveComplete = null;
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

        // Only cancel if we have a valid tween ID and it's actually tweening
        if (rotationTween != -1 && LeanTween.isTweening(rotationTween))
        {
            LeanTween.cancel(rotationTween);
        }

        Debug.Log("Card.Rotate: Rotating card " + gameObject.name + " by " + amount + " degrees");
        rotationTween = LeanTween.rotateAroundLocal(rect, Vector3.up, amount, duration)
            .setOnComplete(() => rotationTween = -1)
            .id;
    }

    public void ForceCompleteMove()
    {
        if (isMoving && movementTween != -1 && LeanTween.isTweening(movementTween))
        {
            LeanTween.cancel(movementTween);
            OnMoveComplete();
        }
    }

    public bool IsMoving()
    {
        return isMoving || (movementTween != -1 && LeanTween.isTweening(movementTween));
    }

    public void MoveToDiscardImmediate()
    {
        ForceCompleteMove();

        GameObject discardPile = GameObject.FindGameObjectWithTag("DiscardPile");
        if (discardPile != null)
        {
            CardHolder discardHolder = discardPile.GetComponent<CardHolder>();
            if (discardHolder != null && discardHolder.Holder != null)
            {
                transform.SetParent(discardHolder.Holder);
                if (rect != null)
                {
                    rect.anchoredPosition3D = discardHolder.Holder.anchoredPosition3D;
                }
            }
        }
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