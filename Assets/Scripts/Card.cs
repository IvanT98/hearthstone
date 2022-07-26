using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Describes a playable card and handles any of it's UI changes.
/// </summary>
public class Card : MonoBehaviour
{
    // UI
    public GameObject cardFront;
    public GameObject cardBack;
    public Image cardImage;
    public TMP_Text cardTitle;
    public TMP_Text cardDescription;
    public TMP_Text cardMana;
    public TMP_Text cardAttack;
    public TMP_Text cardHealth;
    
    private const float CountDownAnimationSpeed = 0.25f;
    private const int MinimumRandomValue = -2;
    private const int MaximumRandomValue = 9;
    private const int MinimumCardHealth = 0;
    
    // Card attributes
    private Sprite _image;
    private string _title = "Title";
    private string _description = "Description";
    private int _mana = 1;
    private int _attack = 1;
    private int _health = 1;

    /// <summary>
    /// Setter for the card image.
    ///
    /// Sets both the card property and the UI.
    /// </summary>
    /// <param name="sprite">New card image.</param>
    public void SetCardImage(Sprite sprite)
    {
        _image = sprite;
        cardImage.sprite = _image;
    }

    /// <summary>
    /// Setter for the card title.
    ///
    /// Sets both the card property and the UI.
    /// </summary>
    /// <param name="title">New card title.</param>
    public void SetCardTitle(string title)
    {
        _title = title;
        cardTitle.SetText(_title);
    }

    /// <summary>
    /// Setter for the card description.
    ///
    /// Sets both the card property and the UI.
    /// </summary>
    /// <param name="description">New card description.</param>
    public void SetCardDescription(string description)
    {
        _description = description;
        cardDescription.SetText(_description);
    }

    /// <summary>
    /// Setter for the card mana.
    ///
    /// Sets both the card property and the UI.
    /// </summary>
    /// <param name="mana">New card mana.</param>
    public void SetCardMana(int mana)
    {
        _mana = mana;
        cardMana.SetText(_mana.ToString());
    }

    /// <summary>
    /// Setter for the card attack.
    ///
    /// Sets both the card property and the UI.
    /// </summary>
    /// <param name="attack">New card attack.</param>
    public void SetCardAttack(int attack)
    {
        _attack = attack;
        cardAttack.SetText(_attack.ToString());
    }
    
    /// <summary>
    /// Setter for the card health.
    ///
    /// Sets both the card property and the UI.
    /// </summary>
    /// <param name="health">New card health.</param>
    public void SetCardHealth(int health)
    {
        _health = health;
        cardHealth.SetText(_health.ToString());
    }

    /// <summary>
    /// Turns the card over.
    /// </summary>
    public void FlipCard()
    {
        var isBackShown = cardBack.activeSelf;

        if (isBackShown)
        {
            cardFront.SetActive(true);
            cardBack.SetActive(false);
            
            return;
        }
        
        cardFront.SetActive(false);
        cardBack.SetActive(true);
    }

    /// <summary>
    /// Sets one random property of the card and executes a countdown animation for the new property value.
    /// </summary>
    /// <returns>IEnumerator to allow waiting for the countdown animation to finish</returns>
    public IEnumerator SetRandomProperty()
    {
        var cardPropertySelector = Utilities.GetRandomNumberInRange(1, 3);
        var randomNumberInRange = Utilities.GetRandomNumberInRange(MinimumRandomValue, MaximumRandomValue);
        var selectedProperty = cardPropertySelector switch
        {
            1 => "_mana",
            2 => "_health",
            _ => "_attack"
        };
        var currentValue = selectedProperty switch
        {
            "_mana" => _mana,
            "_health" => _health,
            _ => _attack
        };

        if (randomNumberInRange == currentValue)
        {
            yield break;
        }
        
        var newValueGreater = randomNumberInRange > currentValue;
        var changePropertyValueCondition =  true;

        while (changePropertyValueCondition)
        {
            currentValue = newValueGreater ? currentValue + 1 : currentValue - 1;

            switch (selectedProperty)
            {
                case "_mana":
                    SetCardMana(currentValue);
                    break;
                case "_health":
                    SetCardHealth(currentValue);
                    break;
                default:
                    SetCardAttack(currentValue);
                    break;
            }

            yield return new WaitForSeconds(CountDownAnimationSpeed);

            changePropertyValueCondition =
                newValueGreater ? currentValue < randomNumberInRange : currentValue > randomNumberInRange;
        }
    }

    /// <summary>
    /// Destroys the card if it's health reaches below the minimum.
    /// </summary>
    private void CheckIfShouldBeDestroyed()
    {
        if (_health > MinimumCardHealth)
        {
            return;
        }
        
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Set the card UI to match the card properties.
    /// </summary>
    private void Start()
    {
        cardTitle.SetText(_title);
        cardDescription.SetText(_description);
        cardMana.SetText(_mana.ToString());
        cardAttack.SetText(_attack.ToString());
        cardHealth.SetText(_health.ToString());
    }

    /// <summary>
    /// Checks if the card should be destroyed.
    /// </summary>
    private void Update()
    {
        CheckIfShouldBeDestroyed();
    }
}
