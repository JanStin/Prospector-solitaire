using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Prospector : MonoBehaviour
{
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

        foreach (CardProspector tempCP in Tableau)
        {
            foreach (int hid in tempCP.SlotDef.HiddenBy)
            {
                cardProspector = FindCardByLayoutID(hid);
                tempCP.HiddenBy.Add(cardProspector);
            }
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
            Layout.Multiplier.x * Layout.DiscardPile.X - 2,
            Layout.Multiplier.y * Layout.DiscardPile.Y + 1.5f,
            -Layout.DiscardPile.LayerID + 0.5f
            );

        card.FaceUp = true;

        card.SetSortingLayerName(Layout.DiscardPile.LayerName);
        card.SetSortOrder(-110 + DiscardPile.Count);
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
            Layout.Multiplier.x * Layout.DiscardPile.X - 2,
            Layout.Multiplier.y * Layout.DiscardPile.Y + 1.5f,
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
            Layout.Multiplier.x * (Layout.DiscardPile.X + 1 + i * dpStagger.x),
            Layout.Multiplier.y * (Layout.DiscardPile.Y + 1 + i * dpStagger.y),
            -Layout.DiscardPile.LayerID + 0.1f * i
            );

            card.FaceUp = false;
            card.State = eCardState.drawpile;

            card.SetSortingLayerName(Layout.DrawPile.LayerName);
            card.SetSortOrder(-10 * i);
        }
    }

    private CardProspector FindCardByLayoutID(int layoutID)
    {
        foreach (CardProspector tempCP in Tableau)
        {
            if (tempCP.LayoutID.Equals(layoutID))
            {
                return tempCP;
            }
        }

        return null;
    }

    private void SetTableauFaces()
    {
        foreach (CardProspector cardProspector in Tableau)
        {
            bool faceUp = true;

            foreach (CardProspector cover in cardProspector.HiddenBy)
            {
                if (cover.State.Equals(eCardState.tableau))
                {
                    faceUp = false;
                }
            }

            cardProspector.FaceUp = faceUp;
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
                ScoreManager.ScoreEvent(eScoreEvent.draw);
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

                MoveToDiscard(Target);
                Tableau.Remove(card);
                MoveToTarget(card);
                SetTableauFaces();
                ScoreManager.ScoreEvent(eScoreEvent.mine);

                break;
        }

        CheckForGameOver();
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

    private void CheckForGameOver()
    {
        if (Tableau.Count.Equals(0))
        {
            GameOver(true);
            return;
        }

        if (DrawPile.Count > 0)
        {
            return;
        }

        // Checking the allowed moves.
        foreach (CardProspector card in Tableau)
        {
            if (AdjacentRank(card, Target))
            {
                return;
            }
        }

        GameOver(false);
    }

    private void GameOver(bool won)
    {
        if (won)
        {
            ScoreManager.ScoreEvent(eScoreEvent.gameWin);
        }
        else
        {
            ScoreManager.ScoreEvent(eScoreEvent.gameLose);
        }

        SceneManager.LoadScene("SampleScene");
    }
}
