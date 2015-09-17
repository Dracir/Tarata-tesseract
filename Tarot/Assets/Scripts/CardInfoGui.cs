using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class CardInfoGui : MonoBehaviour {

    public Text cardNameText;
    public Text cardDescriptionText;

	void Start () {
	
	}
	
	
	void Update () {
	
	}

    public void hideCard()
    {
        gameObject.SetActive(false);
    }

    public void showCard(Card card)
    {
        gameObject.SetActive(true);
        cardNameText.text = card.cardName;
        cardDescriptionText.text = card.description;
    }
}
