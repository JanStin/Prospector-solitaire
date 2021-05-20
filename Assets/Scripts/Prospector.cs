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

            cardProspector.SetSortingLayerName(tempSD.LayerName);

            Tableau.Add(cardProspector);
        }

        MoveToTarget(Draw());
        UpdateDrawPile();
    }

    private void MoveToDiscard(CardProspector card)
    {
        card.State = eCardState.discard;
        DiscardPile.Add(card);
        
        card.transform.parent = LayoutAnchor;
        card.transform.localPosition = new Vector3(
            Layout.Multiplier.x * Layout.DiscardPile.X,
            Layout.Multiplier.y * Layout.DiscardPile.Y,
            -Layout.DiscardPile.LayerID + 0.5f
            );

        card.FaceUp = true;

        card.SetSortingLayerName(Layout.DiscardPile.LayerName);
        card.SetSortOrder(-100 + DiscardPile.Count);
    }

    private void MoveToTarget(CardProspector card)
    {
        if (Target != null)
        {
            MoveToDiscard(card);
        }

        Target = card;
        card.State = eCardState.target;

        card.transform.parent = LayoutAnchor;
        card.transform.localPosition = new Vector3(
            Layout.Multiplier.x * Layout.DiscardPile.X,
            Layout.Multiplier.y * Layout.DiscardPile.Y,
            -Layout.DiscardPile.LayerID
            );

        card.FaceUp = true;
        card.SetSortingLayerName(Layout.DiscardPile.LayerName);
        card.SetSortOrder(0);
    }

    private void UpdateDrawPile()
    {
        CardProspector card;

        for (int i = 0; i < DrawPile.Count; i++)
        {
            card = DrawPile[i];
            card.transform.parent = LayoutAnchor;

            Vector2 dpStagger = Layout.DrawPile.Stagger;

            card.transform.localPosition = new Vector3(
            Layout.Multiplier.x * (Layout.DiscardPile.X + i * dpStagger.x),
            Layout.Multiplier.y * (Layout.DiscardPile.Y + i * dpStagger.y),
            -Layout.DiscardPile.LayerID + 0.1f * i
            );

            card.FaceUp = false;
            card.State = eCardState.drawpile;

            card.SetSortingLayerName(Layout.DrawPile.LayerName);
            card.SetSortOrder(-10 * i);
        }
    }

    public void CardClicked(CardProspector card)
    {
        switch (card.State)
        {
            case eCardState.target:
                // ignore
                break;

            case eCardState.drawpile:
                MoveToDiscard(Target);
                MoveToTarget(Draw());
                UpdateDrawPile();
                break;

            case eCardState.tableau:
                bool validMatch = true;
                
                if (!card.FaceUp)
                {
                    validMatch = false;
                }

                if (!AdjacentRank(card, Target))
                {
                    validMatch = false;
                }

                if (!validMatch)
                {
                    return;
                }

                Tableau.Remove(card);
                MoveToTarget(card);

                break;
        }
    }


    public bool AdjacentRank(CardProspector cardOne, CardProspector cardTwo)
    {
        if (!cardOne.FaceUp || !cardTwo.FaceUp)
        {
            return false;
        }

        if (Mathf.Abs(cardOne.Rank - cardTwo.Rank) == 1)
        {
            return true;
        }

        if (cardOne.Rank.Equals(1) && cardTwo.Rank.Equals(13))
        {
            return true;
        }

        if (cardTwo.Rank.Equals(1) && cardOne.Rank.Equals(13))
        {
            return true;
        }

        return false;
    }
}
