using System;
using UnityEngine;

public class DEBUG : MonoBehaviour
{
    private InventorySystem inventorySystem;
    [SerializeField] private Item[] item;
    [SerializeField] private int[] quantity;
    private void Awake()
    {
        inventorySystem = GetComponent<InventorySystem>();
        
    }

    private void Start()
    {
        for (int i = 0; i < item.Length; i++)
        {
            if (item[i] == null)
            {
                Debug.LogError("Item is null");
                continue;
            }

            if (quantity.Length <= i)
            {
                Debug.LogError("Quantity array is not long enough");
                continue;
            }
            if (quantity[i] <= 0)
            {
                Debug.LogError("Quantity is less than or equal to 0");
                continue;
            }
            inventorySystem.AddItem(item[i], quantity[i]);
        }
    }
}
