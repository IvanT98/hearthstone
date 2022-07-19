using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public GameObject cardDeckGameObject;
    public GameObject cardsContainer;

    private CardDeck _cardDeck;
    private List<GameObject> _playerCards = new List<GameObject>();
    private int _maxCards = 6;
    private bool _wereCardsRetrieved = false;
    private bool _isTakingCard = false;
    private int _cardTransferSpeed = 1;
    private Vector3 _lowestCenterPoint;
    private Vector3 _cardPosition;

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

        float moveOtherCards = _playerCards.Count == 0
            ? 0
            : 25;
        Vector3 cardsContainerPosition = cardsContainer.transform.position;
        Vector3 newCardsContainerPosition = new Vector3(cardsContainerPosition.x - moveOtherCards,
            cardsContainerPosition
                .y, cardsContainerPosition.z);

        cardsContainer.transform.position = newCardsContainerPosition;

        LeanTween.cancel(card);
        _cardPosition = _playerCards.Count == 0
            ? _lowestCenterPoint
            : new Vector3(_cardPosition.x + 25, _cardPosition.y, 0);
        LTDescr move = LeanTween.move(card, _cardPosition, _cardTransferSpeed);
        
        move.setOnComplete(TakeCard, card);
    }

    private void TakeCard(object cardObject)
    {
        GameObject card = cardObject as GameObject;

        if (!card)
        {
            return;
        }
        
        card.transform.SetParent(cardsContainer.transform);
        
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
        
        Renderer renderer = cardsContainer.GetComponent<Renderer>();
        float minY = renderer.bounds.min.y;
        Vector2 gameObjectPosition = cardsContainer.transform.position;

        _lowestCenterPoint = new Vector3(gameObjectPosition.x, minY, 0);
    }

    // Update is called once per frame
    private void Update()
    {
        GetCards();
    }
}
