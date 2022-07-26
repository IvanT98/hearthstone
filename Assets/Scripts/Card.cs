using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Card : MonoBehaviour
{
    public GameObject cardFront;
    public GameObject cardBack;
    public Image cardImage;
    public TMP_Text cardTitle;
    public TMP_Text cardDescription;
    public TMP_Text cardMana;
    public TMP_Text cardAttack;
    public TMP_Text cardHealth;
    
    private Sprite _image;
    private string _title = "Title";
    private string _description = "Description";
    private int _mana = 1;
    private int _attack = 1;
    private int _health = 1;
    
    private const float CountDownAnimationSpeed = 0.25f;

    public void SetCardImage(Sprite sprite)
    {
        _image = sprite;
        cardImage.sprite = _image;
    }

    public void SetCardTitle(string title)
    {
        _title = title;
        cardTitle.SetText(_title);
    }

    public void SetCardDescription(string description)
    {
        _description = description;
        cardDescription.SetText(_description);
    }

    public void SetCardMana(int mana)
    {
        _mana = mana;
        cardMana.SetText(_mana.ToString());
    }

    public void SetCardAttack(int attack)
    {
        _attack = attack;
        cardAttack.SetText(_attack.ToString());
    }
    
    public void SetCardHealth(int health)
    {
        _health = health;
        cardHealth.SetText(_health.ToString());
    }

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

    public IEnumerator SetRandomProperty()
    {
        var cardPropertySelector = Utilities.GetRandomNumberInRange(1, 3);
        var randomNumberInRange = Utilities.GetRandomNumberInRange(-2, 9);
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

    private void CheckIfShouldBeDestroyed()
    {
        if (_health > 0)
        {
            return;
        }
        
        Destroy(gameObject);
    }
    
    private void Start()
    {
        cardTitle.SetText(_title);
        cardDescription.SetText(_description);
        cardMana.SetText(_mana.ToString());
        cardAttack.SetText(_attack.ToString());
        cardHealth.SetText(_health.ToString());
    }

    private void Update()
    {
        CheckIfShouldBeDestroyed();
    }
}
