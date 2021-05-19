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

    public SpriteRenderer[] SpriteRenderers;

    public bool FaceUp
    {
        get
        {
            return !Back.activeSelf;
        }
        set
        {
            Back.SetActive(!value);
        }
    }

    private void Start()
    {
        SetSortOrder(0);
    }

    public void PopulateSpriteRenders()
    {
        if (SpriteRenderers == null || SpriteRenderers.Length == 0)
        {
            SpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }
    }

    // Init pole sortingLayerName in all components SpriteRenderer/.
    public void SetSortingLayerName(string tempSLN)
    {
        PopulateSpriteRenders();

        foreach (SpriteRenderer tempSR in SpriteRenderers)
        {
            tempSR.sortingLayerName = tempSLN;
        }
    }

    public void SetSortOrder(int sortOrder)
    {
        PopulateSpriteRenders();

        foreach (SpriteRenderer tempSR in SpriteRenderers)
        {
            if (tempSR.gameObject == this.gameObject)
            {
                tempSR.sortingOrder = sortOrder;
                continue;
            }
            
            switch (tempSR.gameObject.name)
            {
                case "back":
                    tempSR.sortingOrder = sortOrder + 2;
                    break;

                case "face":
                default:
                    tempSR.sortingOrder = sortOrder + 1;
                    break;
            }
        }
    }

    virtual public void OnMouseUpAsButton()
    {
        print(name);
    }
}
