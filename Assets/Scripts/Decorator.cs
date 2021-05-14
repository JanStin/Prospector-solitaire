using UnityEngine;

[System.Serializable]
public class Decorator
{
    public string Type;
    public Vector3 Location;
    public bool Flip = false; // ������� ���������� ������� �� ���������
    public float Scale = 1f;
}
