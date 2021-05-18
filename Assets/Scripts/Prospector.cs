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
    public float XOffset = 3;
    public float YOffset = -2.5f;
    public Vector3 LayoutCenter;

    [Header("Set Dynamically")]
    public Deck Deck;
    public Layout Layout;
    public List<CardProspector> DrawPile;
    public Transform LayoutAnchor;
    public CardProspector Target;
    public List<CardProspector> Tableau;
    public List<CardProspector> DiscardPile;

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

        LayoutGame();
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

    private CardProspector Draw()
    {
        CardProspector card = DrawPile[0];
        DrawPile.RemoveAt(0);
        return card;
    }

    private void LayoutGame()
    {
        if (LayoutAnchor == null)
        {
            GameObject tempGO = new GameObject("_LayoutAnchor");
            LayoutAnchor = tempGO.transform;
            LayoutAnchor.transform.position = LayoutCenter;
        }

        CardProspector cardProspector;

        foreach (SlotDef tempSD in Layout.SlotDefs)
        {
            cardProspector = Draw();
            cardProspector.FaceUp = tempSD.FaceUp;

            cardProspector.transform.parent = LayoutAnchor;
            cardProspector.transform.localPosition = new Vector3(
                Layout.Multiplier.x * tempSD.X,
                Layout.Multiplier.y * tempSD.Y,
                -tempSD.LayerID
                );

            cardProspector.LayoutID = tempSD.ID;
            cardProspector.SlotDef = tempSD;

            cardProspector.State = eCardState.tableau;
            Tableau.Add(cardProspector);
        }
    }
}
