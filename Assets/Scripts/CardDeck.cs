using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDeck : MonoBehaviour
{
    public GameObject card;

    private int _maxCards = 40;
    private AssetManager _assetManager;
    private List<string> _cardTitles = new List<string>(new []
    {
        "Irondeep Trogg",
        "Murloc Tidecaller",
        "Northshire Cleric",
        "Worgen Infiltrator",
        "Knight of Anointment",
        "Vibrant Squirrel",
        "Voidwalker",
        "Peasant",
        "Voodoo Doctor",
        "Elven Archer",
        "Stockades Guard",
        "Flame Imp",
        "Battlefiend",
        "Abusive Sergeant",
        "Loot Hoarder"
    });
    private List<string> _cardDescriptions = new List<string>(new []
    {
        "After your opponent casts a spell, summon another Irondeep Trogg",
        "Whenever you summon a Murloc, gain +1 Attack",
        "Whenever a minion is healed, draw a card",
        "Stealth",
        "Battlecry: Draw a Holy spell",
        "Deathrattle: Shuffle 4 Acorns into your deck. When drawn, summon a 2/1 Squirrel",
        "Taunt",
        "At the start of your turn, draw a card",
        "Battlecry: Restore 2 Health",
        "Battlecry: Deal 1 damage",
        "Battlecry: Give a friendly minion Taunt",
        "Battlecry: Deal 3 damage to your hero",
        "After your hero attacks, gain +1 Attack",
        "Battlecry: Give a minion +2 Attack this turn",
        "Deathrattle: Draw a card"
    });
    private List<GameObject> _cardsGameObjects;

    public bool WereCardsCreated()
    {
        return _cardsGameObjects.Count >= _maxCards;
    }


    public GameObject TakeCard()
    {
        if (!WereCardsCreated() || _cardsGameObjects.Count == 0)
        {
            return null;
        }

        return null;
    }

    private void PopulateDeck()
    {
        if (!WereCardsCreated() || !_assetManager.WereImagesFetched())
        {
            return;
        }

        for (int i = 0; i < _maxCards; i++)
        {
            GameObject cardInstance = Instantiate(card, new Vector3(0, 0, 0), Quaternion.identity);
            Card cardComponent = cardInstance.GetComponent<Card>();
            
            cardComponent.SetCardTitle(GetCardTitle());
            cardComponent.SetCardDescription(GetCardDescription());
            cardComponent.SetCardImage(_assetManager.GetRandomImage());
            cardComponent.SetCardMana(GetCardMana());
            cardComponent.SetCardAttack(GetCardAttack());
            cardComponent.SetCardHealth(GetCardHealth());
            
            cardInstance.transform.SetParent(gameObject.transform);
            _cardsGameObjects.Add(cardInstance);
        }
    }

    private string GetCardTitle()
    {
        return _cardTitles[Utilities.GetRandomListIndex(_cardTitles.Count)];
    }
    
    private string GetCardDescription()
    {
        return _cardDescriptions[Utilities.GetRandomListIndex(_cardDescriptions.Count)];
    }

    private int GetCardMana()
    {
        return Utilities.GetRandomNumber(10);
    }

    private int GetCardAttack()
    {
        return Utilities.GetRandomNumber(10);
    }

    private int GetCardHealth()
    {
        return Utilities.GetRandomNumber(30);
    }

    // Start is called before the first frame update
    private void Start()
    {
        _assetManager = EventSystem.current.GetComponent(typeof(AssetManager)) as AssetManager;
    }

    private void Update()
    {
        PopulateDeck();
    }
}
