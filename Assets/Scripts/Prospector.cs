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
    public TextAsset LayoutXML;

    [Header("Set Dynamically")]
    public Deck Deck;
    public Layout Layout;
    public List<CardProspector> DrawPile;

    private void Awake()
    {
        S = this;
    }

    private void Start()
    {
        Deck = GetComponent<Deck>();
        Deck.InitDeck(DeckXML.text);
        
        Deck.Shuffle(ref Deck.Cards);

        Layout = GetComponent<Layout>();
        Layout.ReadLayout(LayoutXML.text);
        DrawPile = ConvertListCardToListCardProspector(Deck.Cards);
    }

    private List<CardProspector> ConvertListCardToListCardProspector(List<Card> cards)
    {
        List<CardProspector> cardProspectors = new List<CardProspector>();
        CardProspector temp;

        foreach (Card card in cards)
        {
            temp = card as CardProspector;
            cardProspectors.Add(temp);
        }

        return cardProspectors;
    }
}
