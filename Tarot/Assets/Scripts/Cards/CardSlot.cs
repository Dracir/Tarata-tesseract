using UnityEngine;
using System.Collections;

public class CardSlot : MonoBehaviour {

    public CardDeck sourceDeck;

    public Card currentCard;

    public CardInfoGui cardInfoGui;
    bool showingCard = false;

	void Start () {
	
	}
	
	
	void Update () {
	
	}

    void OnMouseDown() {
        if (currentCard == null)
        {
            currentCard = sourceDeck.GetNextCard();
            currentCard.transform.parent = this.transform;
            currentCard.transform.localPosition = Vector3.zero;
        }
        else
        {
            if (showingCard)
            {
                cardInfoGui.hideCard();
                showingCard = false;
            }
            else
            {
                cardInfoGui.showCard(currentCard);
                showingCard = true;
            }
            
        }
    }
}
