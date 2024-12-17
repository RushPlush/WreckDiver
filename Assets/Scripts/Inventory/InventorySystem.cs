using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[Serializable]
public class ItemRecord
{
    [SerializeField]
    public Item Item = null;
    public int Quantity = 0;
};

public class InventorySystem : MonoBehaviour
{
    [SerializeReference] private List<ItemRecord> items = new();

    /// <summary>
    /// Adds an item to the inventory
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <param name="quantity">The quantity of the item to add.</param>
    public void AddItem(Item item, int quantity)
    {
        var index = items.FindIndex(t => t.Item == item);

        if (index == -1)
        {
            items.Add(new ItemRecord { Item = item, Quantity = quantity });
        }
        else
        {
            items[index].Quantity += quantity;
        }
    }

    /// <summary>
    /// Removes an item from the inventory
    /// </summary>
    /// <param name="item">The item to remove.</param>
    public void RemoveItem(Item item)
    {
        var index = items.FindIndex(t => t.Item == item);

        if (index != -1)
        {
            items[index].Quantity--;
            if (items[index].Quantity <= 0)
            {
                items.RemoveAt(index);
            }
        }
    }

    /// <summary>
    /// Returns the items in the inventory
    /// </summary>
    /// <returns>The items in the inventory.</returns>
    public List<ItemRecord> GetItems()
    {
        return items;
    }

    /// <summary>
    /// Returns the quantity of a specific item in the inventory.
    /// </summary>
    /// <param name="item">The item to retrieve the quantity for.</param>
    /// <returns>The quantity of the item, or 0 if the item is not in the inventory.</returns>
    public int GetQuantity(Item item)
    {
        var index = items.FindIndex(t => t.Item == item);
        return index == -1 ? 0 : items[index].Quantity;
    }

    /// <summary>
    /// Returns true if the given item exists in the inventory, false otherwise.
    /// </summary>
    /// <param name="item">The item to check for.</param>
    /// <returns>True if the item exists in the inventory, false otherwise.</returns>
    public bool HasItem(Item item)
    {
        var index = items.FindIndex(t => t.Item == item);
        return index != -1;
    }

    /// <summary>
    /// Clears the inventory.
    /// </summary>
    public void Clear()
    {
        items = new List<ItemRecord>();
    }

    /// <summary>
    /// Retrieves an item from the inventory at the specified index.
    /// </summary>
    /// <param name="index">The index of the item to retrieve.</param>
    /// <returns>The item at the specified index, or null if the index is out of range.</returns>
    [CanBeNull]
    public Item GetIndex(int index)
    {
        return index >= items.Count ? null : items[index].Item;
    }
}
