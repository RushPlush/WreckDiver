using Inventory;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public string     Name;
    //public Mesh     Mesh;
    //public Material Material;
    public GameObject Prefab;
    public Sprite     Icon;
    public int        Value;
    public bool       HasBehaviour;
    [CanBeNull]public IItem      ItemBehaviour;
}
