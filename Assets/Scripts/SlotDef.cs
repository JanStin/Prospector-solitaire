using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlotDef 
{
    public float X;
    public float Y;
    public bool FaceUp = false;
    public string LayerName = "Default";
    public int LayerID = 0;
    public int ID;
    public List<int> HiddenBy = new List<int>();
    public string Type = "slot";
    public Vector2 Stagger;
}
