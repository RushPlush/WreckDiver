using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public InventorySystem inventorySystem{ get; private set; }
    public Item heldItem { get; private set; } = null;
    [SerializeField] private GameObject itemHolder;
    private GameObject heldItemInstance;
    //private MeshFilter itemHolderMeshFilter;
    
    public bool isHoldingItem { get; private set; }
    private int currentItemIndex = -1;
    private void Awake()
    {
        inventorySystem = GetComponent<InventorySystem>();
        heldItemInstance = itemHolder.transform.GetChild(0).gameObject;
        //itemHolderMeshFilter = itemHolder.GetComponent<MeshFilter>();
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
            heldItemInstance = new GameObject();
            //itemHolderMeshFilter.mesh = null;
            return;
        }
        else
        {
            heldItem = inventorySystem.GetItems()[currentItemIndex].Item;
            //itemHolderMeshFilter.mesh = heldItem.Mesh;
            heldItemInstance = heldItem.Prefab;
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
