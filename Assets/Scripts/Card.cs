using System;
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
        bool isFrontShown = cardFront.activeSelf;

        if (isFrontShown)
        {
            cardFront.SetActive(false);
            cardBack.SetActive(true);
            
            return;
        }
        
        cardFront.SetActive(true);
        cardBack.SetActive(false);
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
}
