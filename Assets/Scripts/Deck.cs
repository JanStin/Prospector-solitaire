using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [Header("Set in Inspector")]
    public bool StartFaceUp = false;

    public Sprite SuitClub;
    public Sprite SuitDiamond;
    public Sprite SuitHeart;
    public Sprite SuitSpade;

    public Sprite[] FaceSprites;
    public Sprite[] RankSprites;

    public Sprite CardBack;
    public Sprite CardBackGold;
    public Sprite CardFrone;
    public Sprite CardFrontGold;

    public GameObject PrefabCard;
    public GameObject PrefabSprite;


    [Header("Set Dynamically")]
    public PT_XMLReader XmlReader;
    public List<string> CardNames;
    public List<Card> Cards;
    public List<Decorator> Decorators;
    public List<CardDefinition> CardDefinitions;
    public Transform DeckAnchor;
    public Dictionary<string, Sprite> DictionarySuits;

    public void InitDeck(string deckXMLText)
    {        
        if (GameObject.Find("_Deck") == null)
        {
            GameObject anchorGO = new GameObject("_Deck");
            DeckAnchor = anchorGO.transform;
        }

        DictionarySuits = new Dictionary<string, Sprite>()
        {
            { "C", SuitClub },
            { "D", SuitDiamond },
            { "H", SuitHeart },
            { "S", SuitSpade }
        };

        ReadDeck(deckXMLText);

        MakeCards();
    }

    public void ReadDeck(string deckXMLText)
    {
        XmlReader = new PT_XMLReader();
        XmlReader.Parse(deckXMLText);

        Decorators = new List<Decorator>();

        PT_XMLHashList xDecos = XmlReader.xml["xml"][0]["decorator"];
        Decorator deco;
        for (int i = 0; i < xDecos.Count; i++)
        {
            deco = new Decorator();
            deco.Type = xDecos[i].att("type");
            deco.Flip = xDecos[i].att("flip").Equals("1");
            deco.Scale = float.Parse(xDecos[i].att("scale"), CultureInfo.InvariantCulture);
            deco.Location.x = float.Parse(xDecos[i].att("x"), CultureInfo.InvariantCulture);
            deco.Location.y = float.Parse(xDecos[i].att("y"), CultureInfo.InvariantCulture);
            deco.Location.z = float.Parse(xDecos[i].att("z"), CultureInfo.InvariantCulture);

            Decorators.Add(deco);
        }

        CardDefinitions = new List<CardDefinition>();

        PT_XMLHashList xCardDefs = XmlReader.xml["xml"][0]["card"];

        CardDefinition cDef;
        for (int i = 0; i < xCardDefs.Count; i++)
        {
            cDef = new CardDefinition();
            cDef.Rank = int.Parse(xCardDefs[i].att("rank"));

            PT_XMLHashList xPips = xCardDefs[i]["pip"];
            if (xPips != null)
            {
                for (int j = 0; j < xPips.Count; j++)
                {
                    deco = new Decorator();
                    deco.Type = "pip";
                    deco.Flip = xPips[j].att("flip").Equals("1");

                    deco.Location.x = float.Parse(xPips[j].att("x"), CultureInfo.InvariantCulture);
                    deco.Location.y = float.Parse(xPips[j].att("y"), CultureInfo.InvariantCulture);
                    deco.Location.z = float.Parse(xPips[j].att("z"), CultureInfo.InvariantCulture);

                    if (xPips[j].HasAtt("scale"))
                    {
                        deco.Scale = float.Parse(xPips[j].att("scale"), CultureInfo.InvariantCulture);
                    }

                    cDef.Pips.Add(deco);
                }
            }

            if (xCardDefs[i].HasAtt("face"))
            {
                cDef.Face = xCardDefs[i].att("face");
            }

            CardDefinitions.Add(cDef);
        }
    }

    public CardDefinition GetCardDefinitionByRank(int rank)
    {
        foreach (CardDefinition card in CardDefinitions)
        {
            if (card.Rank.Equals(rank))
            {
                return card;
            }
        }
        return null;
    }

    public void MakeCards()
    {
        CardNames = new List<string>();
        string[] letters = new string[] { "C", "D", "H", "S" };
        foreach (string s in letters)
        {
            for (int i = 1; i <= 13; i++)
            {
                CardNames.Add(s + i);
            }
        }

        Cards = new List<Card>();
        for (int i = 0; i < CardNames.Count; i++)
        {
            Cards.Add(MakeCard(i));
        }
    }

    private Card MakeCard(int cNum)
    {
        GameObject cardGO = Instantiate(PrefabCard) as GameObject;
        cardGO.transform.parent = DeckAnchor;

        Card card = cardGO.GetComponent<Card>();

        // Cards in row
        cardGO.transform.localPosition = new Vector3((cNum % 13) * 3, cNum / 13 * 4, 0);

        card.name = CardNames[cNum];
        card.Suit = card.name[0].ToString();
        card.Rank = int.Parse(card.name.Substring(1));

        if (card.Suit.Equals("D") || card.Suit.Equals("H"))
        {
            card.ColorName = "Red";
            card.Color = Color.red;
        }

        card.Def = GetCardDefinitionByRank(card.Rank);

        AddDecorators(card);
        AddPips(card);
        AddFace(card);
        AddBack(card);

        return card;
    }

    private Sprite _tSp = null;
    private GameObject _tGO = null;
    private SpriteRenderer _tSR = null;

    private void AddDecorators(Card card)
    {
        foreach (Decorator decorator in Decorators)
        {
            _tGO = Instantiate(PrefabSprite) as GameObject;
            _tSR = _tGO.GetComponent<SpriteRenderer>();

            if (decorator.Type.Equals("suit"))
            {                
                _tSR.sprite = DictionarySuits[card.Suit];
            }
            else
            {
                _tSp = RankSprites[card.Rank];
                _tSR.sprite = _tSp;
                _tSR.color = card.Color;
            }

            _tSR.sortingOrder = 1;

            _tGO.transform.SetParent(card.transform);

            _tGO.transform.localPosition = decorator.Location;

            if (decorator.Flip)
            {
                _tGO.transform.rotation = Quaternion.Euler(0, 0, 180);
            }

            if (decorator.Scale != 1)
            {
                _tGO.transform.localScale = Vector3.one * decorator.Scale;
            }

            _tGO.name = decorator.Type;

            card.DecoratorGOs.Add(_tGO);
        }
    }

    private void AddPips(Card card)
    {
        foreach (Decorator pip in card.Def.Pips)
        {
            _tGO = Instantiate(PrefabSprite) as GameObject; 
            _tSR = _tGO.GetComponent<SpriteRenderer>();

            _tGO.transform.SetParent(card.transform);
            _tGO.transform.localPosition = pip.Location;

            if (pip.Flip)
            {
                _tGO.transform.rotation = Quaternion.Euler(0, 0, 180);
            }

            if (pip.Scale != 1)
            {
                _tGO.transform.localScale = Vector3.one * pip.Scale;
            }

            _tGO.name = "pip";

            _tSR.sprite = DictionarySuits[card.Suit];
            _tSR.sortingOrder = 1;
            card.PipGos.Add(_tGO);
        }
    }

    private void AddFace(Card card)
    {
        if (card.Def.Face == "")
        {
            return;
        }

        _tGO = Instantiate(PrefabSprite) as GameObject;
        _tSR = _tGO.GetComponent<SpriteRenderer>();

        _tSp = GetFace(card.Def.Face + card.Suit);

        _tSR.sprite = _tSp;
        _tSR.sortingOrder = 1;
        _tGO.transform.SetParent(card.transform);
        _tGO.transform.localPosition = Vector3.zero;
        _tGO.name = "face";
    }

    private Sprite GetFace(string face)
    {
        foreach (Sprite sprite in FaceSprites)
        {
            if (sprite.name == face)
            {
                return sprite;
            }
        }

        return null;
    }

    private void AddBack(Card card)
    {
        _tGO = Instantiate(PrefabSprite) as GameObject;
        _tSR = _tGO.GetComponent<SpriteRenderer>();
        
        _tSR.sprite = CardBack;
        
        _tGO.transform.SetParent(card.transform);
        _tGO.transform.localPosition = Vector3.zero;

        _tSR.sortingOrder = 2;

        _tGO.name = "back";
        card.Back = _tGO;

        card.FaceUp = StartFaceUp;
    }

    static public void Shuffle(ref List<Card> cards)
    {
        List<Card> tempCards = new List<Card>();

        int index;
        System.Random random = new System.Random();
        
        while (cards.Count > 0)
        {
            index = random.Next(0, cards.Count);

            tempCards.Add(cards[index]);

            cards.RemoveAt(index);
        }

        cards = tempCards;
    }
}
