using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public GameObject cardDeckGameObject;

    private CardDeck _cardDeck;
    private List<GameObject> _playerCards = new List<GameObject>();
    private int _maxCards = 6;
    private bool _wereCardsRetrieved = false;
    private bool _isTakingACard = false;
    private float _cardsAnimationSpeed = 0.5f;
    private int _cardAnimationSpeed = 1;
    private Vector3 _lastCardPosition;
    private float _cardMargin = 25;
    private Vector3 _cardsPivot;
    private Vector3 _lowerCardsPivot;

    private void TakeCard()
    {
        if (_isTakingACard || _wereCardsRetrieved || !_cardDeck.WereCardsCreated())
        {
            return;
        }

        GameObject card = _cardDeck.TakeCard();

        if (!card)
        {
            _wereCardsRetrieved = true;
            return;
        }

        _isTakingACard = true;

        int playerCardsAmount = _playerCards.Count;
        float existingCardsMovedBy = playerCardsAmount == 0
            ? 0
            : _cardMargin;

        foreach (GameObject storedCard in _playerCards)
        {
            LeanTween.cancel(storedCard);
            LeanTween.move(storedCard,
                new Vector3(storedCard.transform.position.x - existingCardsMovedBy, storedCard.transform.position.y),
                _cardsAnimationSpeed);
        }
        
        _lastCardPosition = playerCardsAmount == 0
            ? _cardsPivot
            : new Vector3(_lastCardPosition.x + _cardMargin, _lastCardPosition.y, 0);
        
        LeanTween.cancel(card);
        LeanTween.move(card, _lastCardPosition, _cardAnimationSpeed).setOnComplete(MoveCard, card);
    }

    private void MoveCard(object cardObject)
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

        RepositionCards();

        _isTakingACard = false;
    }

    private void RepositionCards()
    {
        int addedCardMarginY = 0;

        for (int c = 0; c < _playerCards.Count; c++)
        {
            int cardMarginY = GetCardMarginY(c, _playerCards.Count, 10);
            addedCardMarginY += cardMarginY;
            GameObject playerCard = _playerCards[c];
            Vector3 cardPosition = playerCard.transform.position;
            Vector3 newCardPosition = new Vector3(cardPosition.x, _cardsPivot.y + addedCardMarginY, cardPosition.z);

            LeanTween.cancel(playerCard);
            LeanTween.move(playerCard, newCardPosition, 0.25f).setOnComplete(() =>
            {
                RotateCard(playerCard, 270);
            });
        }
    }

    private int GetCardMarginY(int cardIndex, int totalCards, int cardMargin)
    {
        int cardNumber = cardIndex + 1;

        if (cardNumber == 1)
        {
            return 0;
        }

        bool areCardsEven = totalCards % 2 == 0;
        int[] middleCardsNumber = areCardsEven ? new[] {totalCards / 2, totalCards / 2 + 1} : new [] {totalCards / 2 + 1};

        if (middleCardsNumber.Contains(cardNumber))
        {
            return cardMargin;
        }
        
        int middleCardNumber = middleCardsNumber[0];

        if (cardNumber < middleCardNumber)
        {
            return cardMargin;
        }

        return cardMargin * -1;
    }
    
    
    private void RotateCard(GameObject card, float rotationAdjustment)
    {
        Vector3 relativePos = _lowerCardsPivot - card.transform.position;
        float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg - rotationAdjustment;
        
        card.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // Start is called before the first frame update
    private void Start()
    {
        _cardDeck = cardDeckGameObject.GetComponent<CardDeck>();
        _cardsPivot = new Vector3(gameObject.transform.position.x, -300);
        _lowerCardsPivot = new Vector3(_cardsPivot.x, _cardsPivot.y - 300);
    }

    // Update is called once per frame
    private void Update()
    {
        TakeCard();
    }
}
