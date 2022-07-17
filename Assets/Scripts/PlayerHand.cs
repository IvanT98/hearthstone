using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public GameObject cardDeckGameObject;

    private CardDeck _cardDeck;
    private List<GameObject> _playerCards;
    private bool _wereCardsRetrieved;

    private void GetCards()
    {
        if (!_cardDeck.WereCardsCreated())
        {
            return;
        }
        
        
    }
    
    // Start is called before the first frame update
    private void Start()
    {
        _cardDeck = cardDeckGameObject.GetComponent<CardDeck>();
    }

    // Update is called once per frame
    private void Update()
    {
        GetCards();
    }
}
