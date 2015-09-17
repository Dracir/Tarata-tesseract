using UnityEngine;
using System.Collections;
using System;

public class CardDeck : MonoBehaviour {
    
    public Vector2 deskFoldedSpace;
    [Min(1)]
    public int nbCardSeen = 1;
    public CardShuffleTest cardAnimation;
	
	void Start () {
	
	}
	
	
	void Update () {
	
	}



    public Card GetNextCard()
    {
        GameObject first = transform.GetChild(0).gameObject;
        transform.parent = null;
        cardAnimation.nextCard();
        return first.GetComponent<Card>();
    }
}
