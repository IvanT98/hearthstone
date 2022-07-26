using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Responsible for maintaining and displaying cards in the player's hand.
/// </summary>
public class PlayerHand : MonoBehaviour
{
    public GameObject cardDeckGameObject;

    private CardDeck _cardDeck;
    private readonly List<GameObject> _playerCards = new();
    private const int MinCards = 4;
    private const int MaxCards = 6;
    private int _maxCardsInHand;
    private bool _wereCardsRetrieved;
    private bool _isTakingACard;
    private const float CardTransferAnimationSpeed = 1;
    private const float CardMovementAnimationSpeed = 0.1f;
    private const int CardRotationAdjustmentDegrees = 270;
    private Vector3 _lastCardPosition;
    private const float CardMarginY = 10;
    private Vector3 _cardsMovementPivot;
    private Vector3 _cardsRotationPivot;
    private bool _randomIterationEnabled;
    private int _currentlyIteratedCardIndex = -1;
    private bool _currentlyIterating;

    /// <summary>
    /// Starts the random iteration of card properties.
    /// The iteration goes clockwise, from left to right.
    /// </summary>
    public void StartRandomIteration()
    {
        _randomIterationEnabled = true;
    }
    
    /// <summary>
    /// Performs a single step of random card properties iteration.
    /// </summary>
    /// <returns>IEnumerator to allow waiting for the card animations to complete.</returns>
    private IEnumerator DoRandomIteration()
    {
        if (!_randomIterationEnabled || _currentlyIterating)
        {
            yield break;
        }
        
        _currentlyIterating = true;
        
        var cardGameObject = GetNextCard();
    
        if (!cardGameObject)
        {
            _currentlyIterating = false;
            _randomIterationEnabled = false;
            
            yield break;
        }
    
        var card = cardGameObject.GetComponent<Card>();
        
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
    
    /// <summary>
    /// Gets the next card in the player's hand.
    /// </summary>
    /// <returns>Next card in the player's hand.</returns>
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
        
        var randomCard = _playerCards[_currentlyIteratedCardIndex];
    
        return randomCard;
    }

    /// <summary>
    /// Takes a card from the card deck and transfers it to the player's hand.
    /// </summary>
    /// <returns>IEnumerator to allow waiting for the card animations to complete.</returns>
    private IEnumerator TakeCard()
    {
        if (_isTakingACard || _wereCardsRetrieved || !_cardDeck.WereCardsCreated())
        {
            yield break;
        }
        
        _isTakingACard = true;

        var card = _cardDeck.TakeCard();

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
    
    /// <summary>
    /// Moves card from the card deck to the player's hand.
    /// </summary>
    /// <param name="cardObject">Card taken from the deck.</param>
    /// <returns>IEnumerator to allow waiting for the card animations to complete.</returns>
    private IEnumerator MoveCard(GameObject cardObject)
    {
        var originalCard = cardObject.GetComponent<Card>();
        
        originalCard.FlipCard();

        _playerCards.Add(cardObject);

        yield return RepositionCards();
    }
    
    /// <summary>
    /// Moves the player's hands cards to always keep them centered.
    /// </summary>
    /// <returns>IEnumerator to allow waiting for the card animations to complete.</returns>
    private IEnumerator RepositionCards()
    {
        var playerCardsCopies = new List<GameObject>();

        foreach (var playerCard in _playerCards)
        {
            GameObject playerCardCopy = Instantiate(playerCard, gameObject.transform);
            LayoutElement layoutElement = playerCardCopy.GetComponent<LayoutElement>();

            layoutElement.ignoreLayout = false;
            Card card = playerCardCopy.GetComponent<Card>();
        
            card.cardFront.SetActive(false);
            card.cardBack.SetActive(false);

            playerCardsCopies.Add(playerCardCopy);
        }
        
        yield return new WaitForSeconds(0.1f);

        for (var i = 0; i < playerCardsCopies.Count; i++)
        {
            var playerCardCopy = playerCardsCopies[i];
            var layoutElement = playerCardCopy.GetComponent<LayoutElement>();

            layoutElement.ignoreLayout = true;
            
            var cardMarginY = GetCardMarginY(i);
            var cardPosition = playerCardCopy.transform.position;
            
            playerCardCopy.transform.position = new Vector3(cardPosition.x, cardPosition.y + cardMarginY, 0);
        }

        for (var i = 0; i < playerCardsCopies.Count; i++)
        {
            var playerCard = _playerCards[i];
            var playerCardCopy = playerCardsCopies[i];
            var newCardPosition = playerCardCopy.transform.position;
            var originalCardLayoutElement = playerCard.GetComponent<LayoutElement>();
            
            originalCardLayoutElement.ignoreLayout = true;
            
            LeanTween.cancel(playerCard);

            var playerCardParent = playerCard.transform.parent;
            var belongsToTheDeck = playerCardParent.Equals(gameObject.transform);
            var animationSpeed = belongsToTheDeck ? CardMovementAnimationSpeed : CardTransferAnimationSpeed;

            LeanTween.move(playerCard, newCardPosition, animationSpeed);
            RotateCard(playerCard, newCardPosition, animationSpeed);
            
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

    
    /// <summary>
    /// Calculates the y-axis margin for the given card based on it's position.
    /// </summary>
    /// <param name="cardIndex">The card position index.</param>
    /// <returns>Calculated y-axis margin.</returns>
    private float GetCardMarginY(int cardIndex)
    {
        var totalCards = _playerCards.Count;
        var lastIndex = totalCards - 1;

        if (cardIndex == 0 || cardIndex == lastIndex)
        {
            return 0;
        }

        var middleIndex = totalCards / 2;
        var isNumCardsEven = totalCards % 2 == 0;

        if (isNumCardsEven)
        {
            var additionalMiddleIndex = middleIndex - 1;

            if (cardIndex == additionalMiddleIndex || cardIndex == middleIndex)
            {
                return CardMarginY * additionalMiddleIndex;
            }
        }

        if (cardIndex <= middleIndex)
        {
            return CardMarginY * cardIndex;
        }

        return CardMarginY * (lastIndex - cardIndex);
    }
    
    /// <summary>
    /// Rotates the given card to face the rotation pivot of the player's hand.
    /// </summary>
    /// <param name="card">Card to be rotated.</param>
    /// <param name="cardPosition">Card current or future position. This position will be used to determine by how much the card should be rotated.</param>
    /// <param name="animationSpeed">The card rotation animation speed in seconds.</param>
    private void RotateCard(GameObject card, Vector3 cardPosition, float animationSpeed)
    {
        var relativePos = _cardsRotationPivot - cardPosition;
        var angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg - CardRotationAdjustmentDegrees;
        var angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
        var eulerAngles = angleAxis.eulerAngles;

        LeanTween.rotate(card, eulerAngles, animationSpeed);
    }

    /// <summary>
    /// Sets the required properties of the player's hand.
    /// Determines the number of cards in the player's hand.
    /// </summary>
    private void Start()
    {
        _cardDeck = cardDeckGameObject.GetComponent<CardDeck>();
        _cardsMovementPivot = new Vector3(gameObject.transform.position.x, -300);
        _cardsRotationPivot = new Vector3(_cardsMovementPivot.x, _cardsMovementPivot.y - 300);
        _maxCardsInHand = Utilities.GetRandomNumberInRange(MinCards, MaxCards);
    }

    // Either takes a card from the card deck or performs a random card property value iteration.
    private void Update()
    {
        StartCoroutine(TakeCard());
        StartCoroutine(DoRandomIteration());
    }
}
