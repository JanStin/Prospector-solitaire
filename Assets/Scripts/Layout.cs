using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Layout : MonoBehaviour
{
    public PT_XMLReader Xmlr;
    public PT_XMLHashtable Xml;
    public Vector2 Multiplier;
    public List<SlotDef> SlotDefs = new List<SlotDef>();
    public SlotDef DrawPile;
    public SlotDef DiscardPile;
    public string[] SortingLayerNames = new string[] { "Row0", "Row1", "Row2", "Row3", "Discard", "Draw" };

    public void ReadLayout(string xmlText)
    {
        Xmlr = new PT_XMLReader();
        Xmlr.Parse(xmlText);
        Xml = Xmlr.xml["xml"][0];

        Multiplier.x = float.Parse(Xml["multiplier"][0].att("x"), CultureInfo.InvariantCulture);
        Multiplier.y = float.Parse(Xml["multiplier"][0].att("y"), CultureInfo.InvariantCulture);

        SlotDef tempSD;
        PT_XMLHashList slotsX = Xml["slot"];

        for (int i = 0; i < slotsX.Count; i++)
        {
            tempSD = new SlotDef();
            if (slotsX[i].HasAtt("type"))
            {
                tempSD.Type = slotsX[i].att("type");
            }
            else
            {
                tempSD.Type = "slot";
            }

            tempSD.X = float.Parse(slotsX[i].att("x"), CultureInfo.InvariantCulture);
            tempSD.Y = float.Parse(slotsX[i].att("y"), CultureInfo.InvariantCulture);
            tempSD.LayerID = int.Parse(slotsX[i].att("layer"), CultureInfo.InvariantCulture);
            tempSD.LayerName = SortingLayerNames[tempSD.LayerID];

            switch (tempSD.Type)
            {
                case "slot":
                    tempSD.FaceUp = slotsX[i].att("faceup").Equals("1");
                    tempSD.ID = int.Parse(slotsX[i].att("id"), CultureInfo.InvariantCulture);
                    
                    if (slotsX[i].HasAtt("hiddenby"))
                    {
                        string[] hiding = slotsX[i].att("hiddenby").Split(',');
                        foreach (string s in hiding)
                        {
                            tempSD.HiddenBy.Add(int.Parse(s));
                        }
                    }

                    SlotDefs.Add(tempSD);
                    break;

                case "drawpile":
                    tempSD.Stagger.x = float.Parse(slotsX[i].att("xstagger"), CultureInfo.InvariantCulture);
                    DrawPile = tempSD;
                    break;

                case "discardPile":
                    DiscardPile = tempSD;
                    break;
            }
        }
    }
}
