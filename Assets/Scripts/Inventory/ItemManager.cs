using Inventory;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemManager : MonoBehaviour
{
    public InventorySystem inventorySystem { get; private set; }
    public Item heldItem { get; private set; } = null;
    [SerializeField] private GameObject itemHolder;
    private GameObject heldItemInstance;
    [CanBeNull] private IItem heldItemBehaviour;
    //private MeshFilter itemHolderMeshFilter;

    public bool isHoldingItem { get; private set; }
    private int currentItemIndex = -1;

    public int harpoonsLoaded = 0; // todo save elsewhere, needed for between swaps 
    private void Awake()
    {
        inventorySystem = GetComponent<InventorySystem>();
        // heldItemInstance = itemHolder.transform.GetChild(0).gameObject;
        //itemHolderMeshFilter = itemHolder.GetComponent<MeshFilter>();
    }

    public void ChangeItem(int iteration)
    {
        //print(iteration);
        var count = inventorySystem.GetItems().Count;
        currentItemIndex += iteration;
        Destroy(heldItemInstance);
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
            //Destroy(heldItemInstance);
            heldItemInstance = null;
            //itemHolderMeshFilter.mesh = null;
            return;
        }
        else
        {
            heldItem = inventorySystem.GetItems()[currentItemIndex].Item;
            //itemHolderMeshFilter.mesh = heldItem.Mesh;
            if (heldItem.Prefab != null)
            {
                heldItemInstance = Instantiate(heldItem.Prefab, itemHolder.transform);
            }
            heldItemBehaviour = heldItemInstance.GetComponent<IItem>(); 
            isHoldingItem = true;
            print("Holding: " + heldItem.Name);
        }
    }

    void UseItem()
    {
        if (!isHoldingItem) return;
        if (heldItem == null) return;
        inventorySystem.RemoveItem(heldItem);
    }

    public void PrimaryUse(InputAction.CallbackContext pressed)
    {
        // if(!heldItem.HasBehaviour) return;
        heldItemBehaviour?.PrimaryUse(pressed);
    }

    public void SecondaryUse(InputAction.CallbackContext pressed)
    {
        heldItemBehaviour?.SecondaryUse(pressed);
    }

    public void TertiaryUse(InputAction.CallbackContext pressed)
    {
        heldItemBehaviour?.TertiaryUse(pressed);
    }
}
