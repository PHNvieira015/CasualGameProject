using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class CardDrag : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    bool _dragging;

    Transform _objectToDrag;
    Card _card;
    Vector2 _offset;
    Vector3 _cardSavedPoisition;
    int heightforCardtoPlay = 180;

    void Awake()
    {
        _card = GetComponentInParent<Card>();
        _objectToDrag = _card.transform;
    }

    void Update()
    {
        if (_dragging)
        {
            // Check if the card still exists and is active
            if (_objectToDrag == null || !_objectToDrag.gameObject.activeInHierarchy)
            {
                _dragging = false;
                return;
            }

            _objectToDrag.position = Mouse.current.position.ReadValue() - _offset;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Additional checks before starting drag
        if (_card == null || !_card.CanPlay() || _card.IsBeingPlayed) return;

        // Check if we can queue a card (no card currently being processed)
        PlayCardsState playState = StateMachine.Instance.Current as PlayCardsState;
        if (playState != null && !playState.CanQueueCard()) return;

        _dragging = true;
        _offset = eventData.position - new Vector2(_objectToDrag.position.x, _objectToDrag.position.y);
        _cardSavedPoisition = _card.Rect.anchoredPosition3D;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // If card was destroyed during drag, exit early
        if (_card == null || _objectToDrag == null || _card.IsBeingPlayed)
        {
            _dragging = false;
            return;
        }

        _dragging = false;
        EventSystem.current.SetSelectedGameObject(null);

        if (Mouse.current.position.ReadValue().y >= heightforCardtoPlay && _card.CanPlay())
        {
            // Check if we can queue a card (no card currently being processed)
            PlayCardsState playState = StateMachine.Instance.Current as PlayCardsState;
            if (playState != null && playState.CanQueueCard())
            {
                StateMachine.Instance.CardsdToPlay.Enqueue(_card);
            }
            else
            {
                // Can't queue card right now, return to original position
                _card.Move(_cardSavedPoisition, 0.2f, () => { });
            }
        }
        else
        {
            _card.Move(_cardSavedPoisition, 0.2f, () => { });
        }
    }

    // Force stop dragging when the card is disabled or destroyed
    void OnDisable()
    {
        _dragging = false;
    }

    void OnDestroy()
    {
        _dragging = false;
    }
}