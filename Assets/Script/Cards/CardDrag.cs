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
    int heightforCardtoPlay=180;


    void Awake()
    {
        _card = GetComponentInParent<Card>();
        _objectToDrag = _card.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (_dragging)
        {
           _objectToDrag.position = Mouse.current.position.ReadValue() - _offset;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _dragging = true;
        _offset = eventData.position - new Vector2(_objectToDrag.position.x, _objectToDrag.position.y);
        _cardSavedPoisition = _card.Rect.anchoredPosition3D;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
      _dragging = false;
      EventSystem.current.SetSelectedGameObject(null);


        if(Mouse.current.position.ReadValue().y>=heightforCardtoPlay && _card.CanPlay())

        {
            StateMachine.Instance.CardsdToPlay.Enqueue(_card);
        }
        else
        {
            _card.Move(_cardSavedPoisition, 0.2f, () => { });
        }

    }

}
