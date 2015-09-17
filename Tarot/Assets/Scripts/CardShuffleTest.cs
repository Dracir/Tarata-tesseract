using UnityEngine;
using System.Collections;
using System;
using Magicolo;

public class CardShuffleTest : MonoBehaviour {

    bool shuffle = false;
    public CardDeck deck;

    [Min(0)]
    public float shuffleSpeed;
    float nextNextCardT;

	
	void Start () {
        setUpCardFold();

	}

    [Button("SetupCard", "setUpCardFold")]
    public bool setUp = false;

    private void setUpCardFold()
    {
        int nbInDeck = deck.GetChildCount();
        int nbToShow = Mathf.Min(deck.nbCardSeen, nbInDeck);
        float ratioX = deck.deskFoldedSpace.x / nbToShow;
        float ratioY = deck.deskFoldedSpace.y / nbToShow;
        
        for (int i = 0; i < nbToShow; i++)
        {
            float x = (nbToShow-i - 1) * ratioX;
            float y = (nbToShow-i - 1) * ratioY;
            GameObject cardObject = deck.gameObject.GetChild(i);
            setCardPosition(cardObject, x, y, nbToShow + 1 - i);
        }

        for (int i = nbToShow; i < nbInDeck; i++)
        {
            GameObject cardObject = deck.gameObject.GetChild(i);
            setCardPosition(cardObject, 0, 0, 1);
        }
    }

    private void setCardPosition(GameObject cardObject, float x, float y, int order)
    {
        cardObject.transform.SetLocalPosition(new Vector3(x, y, 0));
        cardObject.GetComponent<Card>().RenderingOrder = order;
    }

    [Button("Next Card", "nextCard")]
    public bool next = false;

    public void nextCard()
    {
        deck.GetChild(0).transform.SetAsLastSibling();
        setUpCardFold();
    }


    void Update () {
        if (shuffle) {
            if (Input.GetMouseButtonUp(0)) {
                shuffle = false;
                endShuffle();
            }
            else
            {
                shuffleCards();
            }
        }
	}

    private void shuffleCards()
    {
        if (nextNextCardT <= 0) {
            nextNextCardT = shuffleSpeed;
            nextCard();
        }
        nextNextCardT -= Time.deltaTime;
    }

    private void endShuffle()
    {
        
    }

    void OnMouseOver() {
        if (Input.GetMouseButtonDown(0)) {
            shuffle = true;
            nextNextCardT = shuffleSpeed;
        }
    }
}
