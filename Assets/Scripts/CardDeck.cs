using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Responsible for creating and maintaining a pool of playable cards.
/// </summary>
public class CardDeck : MonoBehaviour
{
    public GameObject card;

    private const int MaxCards = 40;
    private const int MaxCardRandomMana = 10;
    private const int MaxCardRandomHealth = 30;
    private const int MaxCardRandomAttack = 10;
    private AssetManager _assetManager;
    private readonly List<string> _cardTitles = new(new []
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
    private readonly List<string> _cardDescriptions = new(new []
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
    private readonly List<GameObject> _cardsGameObjects = new();
    private bool _wereCardsCreated;

    /// <summary>
    /// Getter for the _wereCardsCreated flag.
    /// </summary>
    /// <returns>true if maximum amount of cards were created, otherwise false.</returns>
    public bool WereCardsCreated()
    {
        return _wereCardsCreated;
    }

    /// <summary>
    /// Removes one card from the deck.
    /// </summary>
    /// <returns>Removed card instance.</returns>
    public GameObject TakeCard()
    {
        if (!WereCardsCreated() || _cardsGameObjects.Count == 0)
        {
            return null;
        }

        GameObject cardInstance = _cardsGameObjects.Last();
        
        _cardsGameObjects.RemoveAt(_cardsGameObjects.Count - 1);

        return cardInstance;
    }

    
    /// <summary>
    /// Adds cards to the deck.
    /// </summary>
    private void PopulateDeck()
    {
        if (WereCardsCreated() || !_assetManager.WereImagesFetched())
        {
            return;
        }

        var cardInstance = Instantiate(card, new Vector2(0, 0), Quaternion.identity);
        var cardComponent = cardInstance.GetComponent<Card>();
            
        cardComponent.SetCardTitle(GetRandomCardTitle());
        cardComponent.SetCardDescription(GetRandomCardDescription());
        cardComponent.SetCardImage(_assetManager.GetRandomImage());
        cardComponent.SetCardMana(GetRandomCardMana());
        cardComponent.SetCardAttack(GetRandomCardAttack());
        cardComponent.SetCardHealth(GetRandomCardHealth());
            
        cardInstance.transform.SetParent(gameObject.transform);
        
        _cardsGameObjects.Add(cardInstance);
        _wereCardsCreated = _cardsGameObjects.Count >= MaxCards;
    }

    /// <summary>
    /// Getter for random card title.
    /// </summary>
    /// <returns>Random card title.</returns>
    private string GetRandomCardTitle()
    {
        return _cardTitles[Utilities.GetRandomListIndex(_cardTitles.Count)];
    }
    
    /// <summary>
    /// Getter for random card description.
    /// </summary>
    /// <returns>Random card description.</returns>
    private string GetRandomCardDescription()
    {
        return _cardDescriptions[Utilities.GetRandomListIndex(_cardDescriptions.Count)];
    }

    /// <summary>
    /// Getter for random card mana value.
    /// </summary>
    /// <returns>Random card mana value.</returns>
    private static int GetRandomCardMana()
    {
        return Utilities.GetRandomNumber(MaxCardRandomMana);
    }

    /// <summary>
    /// Getter for random card attack value.
    /// </summary>
    /// <returns>Random card attack value.</returns>
    private static int GetRandomCardAttack()
    {
        return Utilities.GetRandomNumber(MaxCardRandomAttack);
    }

    
    /// <summary>
    /// Getter for random card health value.
    /// </summary>
    /// <returns>Random card health value.</returns>
    private static int GetRandomCardHealth()
    {
        return Utilities.GetRandomNumber(MaxCardRandomHealth);
    }
    
    /// <summary>
    /// Retrieves an instance of the asset manager.
    /// </summary>
    private void Start()
    {
        _assetManager = EventSystem.current.GetComponent(typeof(AssetManager)) as AssetManager;
    }

    /// <summary>
    /// Starts card creation.
    /// </summary>
    private void Update()
    {
        PopulateDeck();
    }
}
