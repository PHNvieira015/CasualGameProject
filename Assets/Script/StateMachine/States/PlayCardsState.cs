using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayCardsState : State
{
    public const string PlayedGameObject = "Effects/Played";
    public const string AfterPlayedGameObject = "Effects/AfterPlayed";

    [Header("UI References")]
    [SerializeField] private Button _endTurnButton;
    private string _endTurnButtonPath = "Canvas/EndTurnButton";

    Coroutine _cardSequencer;
    HorizontalLayoutGroup _handLayout;

    private void Awake()
    {
        _handLayout = CardsController.Instance.Hand.Holder.GetComponent<HorizontalLayoutGroup>();
        CacheEndTurnButton();
    }

    private void CacheEndTurnButton()
    {
        if (_endTurnButton == null)
        {
            GameObject buttonObj = GameObject.Find(_endTurnButtonPath);
            if (buttonObj != null)
            {
                _endTurnButton = buttonObj.GetComponent<Button>();
            }

            if (_endTurnButton == null)
            {
                Debug.LogError("End Turn Button not found at path: " + _endTurnButtonPath);
            }
        }
    }

    public override IEnumerator Enter()
    {
        yield return new WaitForSeconds(0.5f);

        CacheEndTurnButton(); // Double-check we have the button reference
        EndTurnButton(true);

        _handLayout.enabled = false;
        _cardSequencer = StartCoroutine(CardSequencer());

        if (_endTurnButton != null)
        {
            _endTurnButton.onClick.AddListener(OnEndTurnClicked);
        }
    }

    public override IEnumerator Exit()
    {
        yield return null;

        EndTurnButton(false);
        _handLayout.enabled = true;

        if (_cardSequencer != null)
        {
            StopCoroutine(_cardSequencer);
        }

        if (_endTurnButton != null)
        {
            _endTurnButton.onClick.RemoveListener(OnEndTurnClicked);
        }
    }

    private void OnEndTurnClicked()
    {
        if (_endTurnButton != null)
        {
            _endTurnButton.interactable = false; // Immediate feedback
        }
        StartCoroutine(DiscardAndEndTurn());
    }

    private IEnumerator DiscardAndEndTurn()
    {
        // Discard all cards in hand
        var hand = CardsController.Instance.Hand;
        while (hand.Cards.Count > 0)
        {
            Card card = hand.Cards[0];
            CardsController.Instance.Discard(card);
            yield return new WaitForSeconds(0.1f);
        }

        // Change state after discarding
        machine.ChangeState<EndTurnState>();
    }

    IEnumerator CardSequencer()
    {
        while (true)
        {
            if (machine.CardsdToPlay.Count > 0)
            {
                Card card = machine.CardsdToPlay.Dequeue();
                Debug.Log("Playing " + card);
                yield return StartCoroutine(PlayCardEffect(card, card.transform.Find(PlayedGameObject)));
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(PlayCardEffect(card, card.transform.Find(AfterPlayedGameObject)));
                yield return new WaitForSeconds(0.5f);
            }
            yield return null;
        }
    }

    IEnumerator PlayCardEffect(Card card, Transform playTransform)
    {
        if (playTransform == null) yield break;

        for (int i = 0; i < playTransform.childCount; i++)
        {
            ITarget targeter = playTransform.GetChild(i).GetComponent<ITarget>();
            List<object> targets = new List<object>();

            if (targeter == null) continue;

            yield return StartCoroutine(targeter.GetTargets(targets));

            foreach (CardEffect effect in playTransform.GetChild(i).GetComponents<CardEffect>())
            {
                yield return StartCoroutine(effect.Apply(targets));
                _handLayout.enabled = true;
            }
        }
    }

    void EndTurnButton(bool interactable)
    {
        if (_endTurnButton == null)
        {
            CacheEndTurnButton();

            if (_endTurnButton == null)
            {
                Debug.LogWarning("Failed to find End Turn Button");
                return;
            }
        }

        _endTurnButton.interactable = interactable;
    }
}