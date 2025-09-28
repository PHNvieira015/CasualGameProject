using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;

public class CardDrag : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IEventSystemHandler
{
    bool _dragging;

    Transform _objectToDrag;
    Card _card;
    Vector2 _offset;
    Vector3 _cardSavedPoisition;
    int heightforCardtoPlay = 180;
    private static bool _canSelect = true;
    private static float animationWaitTime = 0.5f;

    void Awake()
    {
        _card = GetComponentInParent<Card>();
        _objectToDrag = _card.transform;
    }

    void Update()
    {
        // Always reset dragging if mouse button is not pressed
        if (_dragging && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            _dragging = false;
            EventSystem.current.SetSelectedGameObject(null);
            _card.Move(_cardSavedPoisition, 0.2f, () => { });
        }

        if (_dragging)
        {
            _objectToDrag.position = Mouse.current.position.ReadValue() - _offset;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_canSelect) return;
        _dragging = true;
        _offset = eventData.position - new Vector2(_objectToDrag.position.x, _objectToDrag.position.y);
        _cardSavedPoisition = _card.Rect.anchoredPosition3D;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_dragging) return;
        _dragging = false;
        EventSystem.current.SetSelectedGameObject(null);

        if (Mouse.current.position.ReadValue().y >= heightforCardtoPlay && _card.CanPlay())
        {
            if (StateMachine.Instance.CardsdToPlay.Count == 0)
            {
                StateMachine.Instance.CardsdToPlay.Enqueue(_card);
                StartCoroutine(BlockSelectionForAnimation(animationWaitTime));
            }
        }
        else
        {
            _card.Move(_cardSavedPoisition, 0.2f, () => { });
        }
    }

    private IEnumerator BlockSelectionForAnimation(float delay)
    {
        _canSelect = false;
        yield return new WaitForSeconds(delay);
        _canSelect = true;
    }

    // Add this static method to allow selection
    public static void AllowSelection()
    {
        _canSelect = true;
    }
}
