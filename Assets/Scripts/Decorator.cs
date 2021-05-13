using UnityEngine;

[System.Serializable]
public class Decorator
{
    public string Type { get; set; }
    public Vector3 Location { get; set; }
    public bool Flip = false; // ѕризнак переворота спрайта по вертикале
    public readonly float Scale = 1f;
}
