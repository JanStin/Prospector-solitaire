using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [Header("Set Dynamically")]
    public string Suit;
    public int Rank;
    public Color Color = Color.black;
    public string ColorName = "Black";
    
    public List<GameObject> DecoratorGOs = new List<GameObject>();
    public List<GameObject> PipGos = new List<GameObject>();

    public GameObject Back;
    public CardDefinition Def;
}
