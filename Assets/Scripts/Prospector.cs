using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Prospector : MonoBehaviour
{
    // TODO: rename this variable;
    static public Prospector S;

    [Header("Set in Inspector")]
    public TextAsset DeckXML;

    [Header("Set Dynamically")]
    public Deck Deck;

    private void Awake()
    {
        S = this;
    }

    private void Start()
    {
        Deck = GetComponent<Deck>();
        Deck.InitDeck(DeckXML.text);
        
        Deck.Shuffle(ref Deck.Cards);

        Card tempCard;
        for (int i = 0; i < Deck.Cards.Count; i++)
        {
            tempCard = Deck.Cards[i];
            tempCard.transform.localPosition = new Vector3(i % 13 * 3, i / 13 * 4, 0);
        }
    }
}
