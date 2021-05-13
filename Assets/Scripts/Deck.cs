using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [Header("Set Dynamically")]
    public PT_XMLReader xmlReader;

    public void InitDeck(string deckXMLText)
    {
        ReadDeck(deckXMLText);
    }

    public void ReadDeck(string deckXMLText)
    {
        xmlReader = new PT_XMLReader();
        xmlReader.Parse(deckXMLText);

        // Output for test.
        string s = $"xml[0] decoration[0] type={xmlReader.xml["xml"][0]["decorator"][0].att("type")}" +
            $"\nx={xmlReader.xml["xml"][0]["decorator"][0].att("x")}" +
            $"\ny={xmlReader.xml["xml"][0]["decorator"][0].att("y")}";
        print(s);
    }
}
