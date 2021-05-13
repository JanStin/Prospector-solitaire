using System.Collections.Generic;

[System.Serializable]
public class CardDefinition
{
    public string Face { get; set; }
    public int Rank { get; set; }
    public List<Decorator> Pips = new List<Decorator>();
}
