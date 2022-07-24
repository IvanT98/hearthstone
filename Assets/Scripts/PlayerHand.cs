using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public GameObject cardDeckGameObject;

    private CardDeck _cardDeck;
    private List<GameObject> _playerCards = new List<GameObject>();
    private int _maxCards = 6;
    private bool _wereCardsRetrieved = false;
    private bool _isTakingACard = false;
    private float _cardTransferAnimationSpeed = 1;
    private float _cardMovementAnimationSpeed = 0.1f;
    private Vector3 _lastCardPosition;
    private float _cardMarginX = 25;
    private float _cardMarginY = 10;
    private Vector3 _cardsMovementPivot;
    private Vector3 _cardsRotationPivot;

    private IEnumerator TakeCard()
    {
        if (_isTakingACard || _wereCardsRetrieved || !_cardDeck.WereCardsCreated())
        {
            yield break;
        }

        GameObject card = _cardDeck.TakeCard();

        if (!card)
        {
            _wereCardsRetrieved = true;
            
            yield break;
        }

        _isTakingACard = true;

        yield return MoveCard(card);
    }
    
    private IEnumerator MoveCard(GameObject card)
    {
        yield return RepositionCards(_playerCards.Count + 1);

        Vector3 nextCardPosition = GetCardPosition(_playerCards.Count, _playerCards.Count + 1);

        LeanTween.cancel(card);
        LeanTween.move(card, nextCardPosition, _cardTransferAnimationSpeed);
        RotateCard(card, nextCardPosition, 270, _cardTransferAnimationSpeed);

        yield return new WaitForSeconds(_cardTransferAnimationSpeed);
        
        card.transform.SetParent(gameObject.transform);
        
        Card cardComponent = card.GetComponent<Card>();
        
        cardComponent.FlipCard();
        
        _playerCards.Add(card);
        
        _wereCardsRetrieved = _playerCards.Count >= _maxCards;
        _isTakingACard = false;
    }

    private IEnumerator RepositionCards(int totalCards)
    {
        for (int c = 0; c < _playerCards.Count; c++)
        {
            GameObject playerCard = _playerCards[c];
            Vector3 newCardPosition = GetCardPosition(c, totalCards);

            LeanTween.cancel(playerCard);
            LeanTween.move(playerCard, newCardPosition, _cardMovementAnimationSpeed);
            RotateCard(playerCard, playerCard.transform.position, 270, _cardMovementAnimationSpeed);
            
            yield return new WaitForSeconds(_cardMovementAnimationSpeed);
        }
    }

    private Vector3 GetCardPosition(int cardIndex, int totalCards)
    {
        int cardsAmount = _playerCards.Count;
        Vector3 lastCardPosition;
        
        if (cardsAmount == 0)
        {
            lastCardPosition = _cardsMovementPivot;
        }
        else
        {
            GameObject previousCard = cardIndex == 0 ? _playerCards[0] : _playerCards[cardIndex - 1];
            lastCardPosition = previousCard.transform.position;
        }
        
        float addedCardMarginY = GetCardMarginY(cardIndex, totalCards, _cardMarginY);
        float cardPositionMarginX = cardsAmount == 0
            ? 0
            : cardIndex == 0 ? _cardMarginX : _cardMarginX * 2;
        int direction = cardIndex == 0 ? -1 : 1;
        float newCardPositionX = lastCardPosition.x + cardPositionMarginX * direction;
        float newCardPositionY = _cardsMovementPivot.y + addedCardMarginY;

        return new Vector3(newCardPositionX, newCardPositionY, lastCardPosition.z);
    }
    
    private float GetCardMarginY(int cardIndex, int totalCards, float cardMargin)
    {
        int lastIndex = totalCards - 1;

        if (cardIndex == 0 || cardIndex == lastIndex)
        {
            return 0;
        }

        int middleIndex = totalCards / 2;
        bool isNumCardsEven = totalCards % 2 == 0;

        if (isNumCardsEven)
        {
            int additionalMiddleIndex = middleIndex - 1;

            if (cardIndex == additionalMiddleIndex || cardIndex == middleIndex)
            {
                return cardMargin * additionalMiddleIndex;
            }
        }

        if (cardIndex <= middleIndex)
        {
            return cardMargin * cardIndex;
        }

        return cardMargin * (lastIndex - cardIndex);
    }
    
    
    private void RotateCard(GameObject card, Vector3 cardPosition, float rotationAdjustment, float animationSpeed)
    {
        Vector3 relativePos = _cardsRotationPivot - cardPosition;
        float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg - rotationAdjustment;
        Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
        Vector3 eulerAngles = angleAxis.eulerAngles;

        LeanTween.rotate(card, eulerAngles, animationSpeed);
    }

    // Start is called before the first frame update
    private void Start()
    {
        _cardDeck = cardDeckGameObject.GetComponent<CardDeck>();
        _cardsMovementPivot = new Vector3(gameObject.transform.position.x, -300);
        _cardsRotationPivot = new Vector3(_cardsMovementPivot.x, _cardsMovementPivot.y - 300);
    }

    // Update is called once per frame
    private void Update()
    {
        StartCoroutine(TakeCard());
    }
}
