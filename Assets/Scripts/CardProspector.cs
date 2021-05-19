using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardProspector : Card
{
    [Header("Set Dynamiclally: CardProspector")]
    public eCardState State = eCardState.drawpile;
    public List<CardProspector> HiddenBy = new List<CardProspector>();
    public int LayoutID;
    public SlotDef SlotDef;

    public override void OnMouseUpAsButton()
    {
        Prospector.S.CardClicked(this);
        base.OnMouseUpAsButton();
    }
}
