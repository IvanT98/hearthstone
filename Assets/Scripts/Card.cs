using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    // Card properties
    private Sprite _image;
    private string _title = "Title";
    private string _description = "Description";
    private int _mana = 1;
    private int _attack = 1;
    private int _health = 1;

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
        bool isBackShown = cardBack.activeSelf;

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
        float countDownSpeed = 0.25f;
        int cardPropertySelector = Utilities.GetRandomNumberInRange(1, 3);
        int randomNumberInRange = Utilities.GetRandomNumberInRange(-2, 9);

        if (cardPropertySelector == 1)
        {
            if (randomNumberInRange == _mana)
            {
                yield break;
            }
            
            bool newValueGreater = randomNumberInRange > _mana;

            if (newValueGreater)
            {
                while (_mana < randomNumberInRange)
                {
                    _mana += 1;
                    SetCardMana(_mana);

                    yield return new WaitForSeconds(countDownSpeed);
                }
            }
            else
            {
                while (_mana > randomNumberInRange)
                {
                    _mana -= 1;
                    SetCardMana(_mana);

                    yield return new WaitForSeconds(countDownSpeed);
                }
            }
            
            yield break;
        }

        if (cardPropertySelector == 2)
        {
            if (randomNumberInRange == _health)
            {
                yield break;
            }
            
            bool newValueGreater = randomNumberInRange > _health;

            if (newValueGreater)
            {
                while (_health < randomNumberInRange)
                {
                    _health += 1;
                    SetCardHealth(_health);

                    yield return new WaitForSeconds(countDownSpeed);
                }
            }
            else
            {
                while (_health > randomNumberInRange)
                {
                    _health -= 1;
                    SetCardHealth(_health);

                    yield return new WaitForSeconds(countDownSpeed);
                }
            }
            
            yield break;
        }
        
        if (randomNumberInRange == _attack)
        {
            yield break;
        }
            
        bool IsNewValueGreater = randomNumberInRange > _attack;

        if (IsNewValueGreater)
        {
            while (_attack < randomNumberInRange)
            {
                _attack += 1;
                SetCardAttack(_attack);

                yield return new WaitForSeconds(countDownSpeed);
            }
        }
        else
        {
            while (_attack > randomNumberInRange)
            {
                _attack -= 1;
                SetCardAttack(_attack);

                yield return new WaitForSeconds(countDownSpeed);
            }
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

    // Start is called before the first frame update
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
