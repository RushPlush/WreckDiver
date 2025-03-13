using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public string   Name;
    public Mesh     Mesh;
    public Material Material;
    public Sprite   Icon;
    public int      Value;
}
