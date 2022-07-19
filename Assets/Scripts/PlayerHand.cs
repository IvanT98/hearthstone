using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public GameObject cardDeckGameObject;

    private CardDeck _cardDeck;
    private List<GameObject> _playerCards = new List<GameObject>();
    private int _maxCards = 6;
    private bool _wereCardsRetrieved = false;
    private bool _isTakingCard = false;
    private int _cardTransferSpeed = 1;
    private Vector2 _lowerCenter;

    private void GetCards()
    {
        if (_isTakingCard || _wereCardsRetrieved || !_cardDeck.WereCardsCreated())
        {
            return;
        }

        GameObject card = _cardDeck.TakeCard();

        if (!card)
        {
            _wereCardsRetrieved = true;
            return;
        }

        _isTakingCard = true;

        LeanTween.cancel(card);
        LTDescr move = LeanTween.move(card, _lowerCenter, _cardTransferSpeed);
        
        move.setOnComplete(AssignCard, card);
    }

    private void AssignCard(object cardObject)
    {
        GameObject card = cardObject as GameObject;

        if (!card)
        {
            return;
        }
        
        card.transform.SetParent(gameObject.transform);
        
        Card cardComponent = card.GetComponent<Card>();
        
        cardComponent.FlipCard();
        
        _playerCards.Add(card);
        
        _wereCardsRetrieved = _playerCards.Count >= _maxCards;

        _isTakingCard = false;
    }

    // Start is called before the first frame update
    private void Start()
    {
        _cardDeck = cardDeckGameObject.GetComponent<CardDeck>();
        _lowerCenter = gameObject.transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        GetCards();
    }
}
