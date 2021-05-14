using System.Collections.Generic;

[System.Serializable]
public class CardDefinition
{
    public string Face;
    public int Rank;
    public List<Decorator> Pips = new List<Decorator>();
}
