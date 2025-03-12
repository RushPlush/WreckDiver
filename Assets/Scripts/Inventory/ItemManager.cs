using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private InventorySystem inventorySystem;
    public Item heldItem { get; private set; } = null;
    [SerializeField] private GameObject itemHolder;
    private MeshFilter itemHolderMeshFilter;
    
    public bool isHoldingItem { get; private set; }
    private int currentItemIndex = -1;
    private void Awake()
    {
        inventorySystem = GetComponent<InventorySystem>();
        itemHolderMeshFilter = itemHolder.GetComponent<MeshFilter>();
    }
    
    void ChangeItem(int iteration)
    {
        var count = inventorySystem.GetItems().Count;
        currentItemIndex += iteration;
        if (currentItemIndex < 0)
        {
            currentItemIndex = count - 1;
        }
        else if (currentItemIndex >= count)
        {
            currentItemIndex = 0;
        }
        if (count == 0)
        {
            currentItemIndex = -1;
            heldItem = null;
            isHoldingItem = false;
            itemHolderMeshFilter.mesh = null;
            return;
        }
        else
        {
            heldItem = inventorySystem.GetItems()[currentItemIndex].Item;
            itemHolderMeshFilter.mesh = heldItem.Mesh;
            isHoldingItem = true;
        }
    }

    void UseItem()
    {
        if(!isHoldingItem) return;
        if(heldItem == null) return;
        inventorySystem.RemoveItem(heldItem);
    }
}
