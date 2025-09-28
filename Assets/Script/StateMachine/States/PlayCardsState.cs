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
    private string _endTurnButtonPath = "Canvas/Menus/CombatScreen/EndTurnButton";

    Coroutine _cardSequencer;
    HorizontalLayoutGroup _handLayout;
    private bool _isProcessingCard = false; // Track if we're currently processing a card

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
        _isProcessingCard = false; // Reset processing state
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
        _isProcessingCard = false; // Reset processing state

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
        // Stop processing cards before discarding
        if (_cardSequencer != null)
        {
            StopCoroutine(_cardSequencer);
            _cardSequencer = null;
        }

        _isProcessingCard = false; // Reset processing state

        // Disable all CardDrag components in hand to prevent stuck cards
        var hand = CardsController.Instance.Hand;

        // Create a copy of the list to avoid modification during iteration
        List<Card> cardsToDiscard = new List<Card>(hand.Cards);

        foreach (Card card in cardsToDiscard)
        {
            // Disable CardDrag for this specific card
            CardDrag cardDrag = card.GetComponentInChildren<CardDrag>();
            if (cardDrag != null)
            {
                cardDrag.enabled = false;
            }

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
            if (machine.CardsdToPlay.Count > 0 && !_isProcessingCard)
            {
                _isProcessingCard = true; // Start processing a card

                Card card = machine.CardsdToPlay.Dequeue();

                // Check if the card still exists and is valid
                if (card == null || card.transform == null)
                {
                    _isProcessingCard = false; // Reset if card is invalid
                    continue;
                }

                Debug.Log("Playing " + card);

                // Mark card as being played to prevent double play
                card.SetBeingPlayed(true);

                // Disable the CardDrag component immediately when playing starts
                CardDrag cardDrag = card.GetComponentInChildren<CardDrag>();
                if (cardDrag != null)
                {
                    cardDrag.enabled = false;
                }

                // Remove from hand immediately
                CardsController.Instance.Hand.RemoveCard(card);

                // Check if the Played transform exists
                Transform playedTransform = card.transform.Find(PlayedGameObject);
                if (playedTransform != null)
                {
                    yield return StartCoroutine(PlayCardEffect(card, playedTransform));
                    yield return new WaitForSeconds(0.5f);
                }

                // Check if the AfterPlayed transform exists
                Transform afterPlayedTransform = card.transform.Find(AfterPlayedGameObject);
                if (afterPlayedTransform != null)
                {
                    yield return StartCoroutine(PlayCardEffect(card, afterPlayedTransform));
                    yield return new WaitForSeconds(0.5f);
                }

                // Discard the card after all effects are complete
                CardsController.Instance.Discard(card);

                _isProcessingCard = false; // Finished processing this card
            }
            yield return null;
        }
    }

    IEnumerator PlayCardEffect(Card card, Transform playTransform)
    {
        // Check if the card or playTransform has been destroyed
        if (card == null || playTransform == null) yield break;

        int childCount = playTransform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            if (i >= playTransform.childCount) yield break;

            Transform child = playTransform.GetChild(i);
            if (child == null) continue;

            ITarget targeter = child.GetComponent<ITarget>();
            List<object> targets = new List<object>();

            if (targeter == null) continue;

            yield return StartCoroutine(targeter.GetTargets(targets));

            if (child == null) continue;

            CardEffect[] effects = child.GetComponents<CardEffect>();
            foreach (CardEffect effect in effects)
            {
                if (effect != null)
                {
                    yield return StartCoroutine(effect.Apply(targets));
                }
            }
        }

        _handLayout.enabled = true;
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

    // Public method to check if we can queue a card
    public bool CanQueueCard()
    {
        return !_isProcessingCard && machine.CardsdToPlay.Count == 0;
    }
}