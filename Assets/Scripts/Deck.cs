using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [Header("Set Dynamically")]
    public PT_XMLReader XmlReader;
    public List<string> CardNames;
    public List<Card> Cards;
    public List<Decorator> Decorators;
    public List<CardDefinition> CardDefinitions;
    public Transform DeckAnchor;
    public Dictionary<string, Sprite> DictSuits;

    public void InitDeck(string deckXMLText)
    {
        ReadDeck(deckXMLText);
    }

    public void ReadDeck(string deckXMLText)
    {
        XmlReader = new PT_XMLReader();
        XmlReader.Parse(deckXMLText);

        // Output for test.
        //string s = $"xml[0] decoration[0] type={XmlReader.xml["xml"][0]["decorator"][0].att("type")}" +
        //    $"\nx={XmlReader.xml["xml"][0]["decorator"][0].att("x")}" +
        //    $"\ny={XmlReader.xml["xml"][0]["decorator"][0].att("y")}" +
        //    $"\nscale={XmlReader.xml["xml"][0]["decorator"][0].att("scale")}";
        //print(s);

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
                    deco.Flip = xPips[i].att("flip").Equals("1");

                    deco.Location.x = float.Parse(xPips[i].att("x"), CultureInfo.InvariantCulture);
                    deco.Location.y = float.Parse(xPips[i].att("y"), CultureInfo.InvariantCulture);
                    deco.Location.z = float.Parse(xPips[i].att("z"), CultureInfo.InvariantCulture);

                    if (xPips[j].HasAtt("scale"))
                    {
                        deco.Scale = float.Parse(xPips[i].att("scale"), CultureInfo.InvariantCulture);
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
}
