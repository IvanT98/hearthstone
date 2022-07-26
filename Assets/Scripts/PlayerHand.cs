using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHand : MonoBehaviour
{
    public GameObject cardDeckGameObject;

    private CardDeck _cardDeck;
    private List<GameObject> _playerCards = new List<GameObject>();
    private int _minCards = 4;
    private int _maxCards = 6;
    private int _maxCardsInHand;
    private bool _wereCardsRetrieved = false;
    private bool _isTakingACard = false;
    private float _cardTransferAnimationSpeed = 1;
    private float _cardMovementAnimationSpeed = 0.1f;
    private Vector3 _lastCardPosition;
    private float _cardMarginX = 25;
    private float _cardMarginY = 10;
    private Vector3 _cardsMovementPivot;
    private Vector3 _cardsRotationPivot;
    private bool _randomIterationEnabled = false;
    private int _currentlyIteratedCardIndex = -1;
    private bool _currentlyIterating = false;

    public void StartRandomIteration()
    {
        _randomIterationEnabled = true;
    }
    
    private IEnumerator DoRandomIteration()
    {
        if (!_randomIterationEnabled || _currentlyIterating)
        {
            yield break;
        }
        
        _currentlyIterating = true;
        
        GameObject cardGameObject = GetNextCard();
    
        if (!cardGameObject)
        {
            _currentlyIterating = false;
            _randomIterationEnabled = false;
            
            yield break;
        }
    
        Card card = cardGameObject.GetComponent<Card>();
        
        yield return card.SetRandomProperty();
    
        if (!card)
        {
            _playerCards.RemoveAt(_currentlyIteratedCardIndex);
    
            _currentlyIteratedCardIndex -= 1;
    
            if (_currentlyIteratedCardIndex < 0)
            {
                _currentlyIteratedCardIndex = 0;
            }
    
            yield return RepositionCards();
        }
    
        _currentlyIterating = false;
    }
    
    private GameObject GetNextCard()
    {
        if (_playerCards.Count == 0)
        {
            _currentlyIteratedCardIndex = -1;
            
            return null;
        }
    
        _currentlyIteratedCardIndex += 1;
    
        if (_currentlyIteratedCardIndex >= _playerCards.Count)
        {
            _currentlyIteratedCardIndex = 0;
        }
        
        GameObject randomCard = _playerCards[_currentlyIteratedCardIndex];
    
        return randomCard;
    }

    private IEnumerator TakeCard()
    {
        if (_isTakingACard || _wereCardsRetrieved || !_cardDeck.WereCardsCreated())
        {
            yield break;
        }
        
        _isTakingACard = true;

        GameObject card = _cardDeck.TakeCard();

        if (!card)
        {
            _wereCardsRetrieved = true;
            _isTakingACard = false;
            
            yield break;
        }

        yield return MoveCard(card);
        
        _wereCardsRetrieved = _playerCards.Count > _maxCardsInHand;
        _isTakingACard = false;
    }
    
    private IEnumerator MoveCard(GameObject cardObject)
    {
        Card originalCard = cardObject.GetComponent<Card>();
        
        originalCard.FlipCard();

        _playerCards.Add(cardObject);

        yield return RepositionCards();
    }
    
    private IEnumerator RepositionCards()
    {
        List<GameObject> playerCardsCopies = new List<GameObject>();

        for (int i = 0; i < _playerCards.Count; i++)
        {
            GameObject playerCard = _playerCards[i];
            GameObject playerCardCopy = Instantiate(playerCard, gameObject.transform);
            LayoutElement layoutElement = playerCardCopy.GetComponent<LayoutElement>();

            layoutElement.ignoreLayout = false;
            Card card = playerCardCopy.GetComponent<Card>();
        
            card.cardFront.SetActive(false);
            card.cardBack.SetActive(false);

            playerCardsCopies.Add(playerCardCopy);
        }
        
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < playerCardsCopies.Count; i++)
        {
            GameObject playerCardCopy = playerCardsCopies[i];
            LayoutElement layoutElement = playerCardCopy.GetComponent<LayoutElement>();

            layoutElement.ignoreLayout = true;
            
            float cardMarginY = GetCardMarginY(i, _playerCards.Count, _cardMarginY);
            Vector3 cardPosition = playerCardCopy.transform.position;
            
            playerCardCopy.transform.position = new Vector3(cardPosition.x, cardPosition.y + cardMarginY, 0);
        }

        for (int i = 0; i < playerCardsCopies.Count; i++)
        {
            GameObject playerCard = _playerCards[i];
            GameObject playerCardCopy = playerCardsCopies[i];
            Vector3 newCardPosition = playerCardCopy.transform.position;
            LayoutElement originalCardLayoutElement = playerCard.GetComponent<LayoutElement>();
            
            originalCardLayoutElement.ignoreLayout = true;
            
            LeanTween.cancel(playerCard);

            Transform playerCardParent = playerCard.transform.parent;
            bool belongsToTheDeck = playerCardParent.Equals(gameObject.transform);
            float animationSpeed = belongsToTheDeck ? _cardMovementAnimationSpeed : _cardTransferAnimationSpeed;

            LeanTween.move(playerCard, newCardPosition, _cardTransferAnimationSpeed);
            RotateCard(playerCard, newCardPosition, 270, _cardTransferAnimationSpeed);
            
            yield return new WaitForSeconds(animationSpeed);

            if (!belongsToTheDeck)
            {
                playerCard.transform.SetParent(gameObject.transform);
            }
        }

        foreach (var playerCardsCopy in playerCardsCopies)
        {
            Destroy(playerCardsCopy);
        }
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
        _maxCardsInHand = Utilities.GetRandomNumberInRange(_minCards, _maxCards);
    }

    // Update is called once per frame
    private void Update()
    {
        StartCoroutine(TakeCard());
        StartCoroutine(DoRandomIteration());
    }
}
