using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveAndLoad : MonoBehaviour
{
    Vector3 playerPosition;
    InventorySystem inventorySystem;
    private List<ItemRecord> items;
    ItemManager itemManager;
    int numberOfHarpoons;
    OxygenBehaviour oxygenBehaviour;
    private void Awake()
    {
        inventorySystem = GetComponent<InventorySystem>();
        itemManager     = GetComponent<ItemManager>();
        oxygenBehaviour = GetComponent<OxygenBehaviour>();
    }
    private void Start()
    {
        Save();
    }
    public void Save()
    {
        items = inventorySystem.GetItems();
        playerPosition = transform.position;
        numberOfHarpoons = itemManager.harpoonsLoaded;
        print("Saved player position: " + playerPosition);
    }
    public void Load()
    {
        inventorySystem.Clear();
        inventorySystem.SetItems(items);
        itemManager.ChangeItem(0); // Making sure that the player is holding a valid item, whilst trying to preserve the last item the player was holding
        oxygenBehaviour.Reset();
        transform.position = playerPosition;
        itemManager.harpoonsLoaded = numberOfHarpoons;
        var shotHarpoons = FindObjectsByType<Harpoon>(FindObjectsSortMode.None);
        foreach (var harpoon in shotHarpoons)
        {
            harpoon.Reset();
        }
    }

    public void Death()
    {
        // possibly add cause of death, example from falling or drowning
        //todo: insert death animation, Death screen, sound etc
        print($"Player died at {transform.position} with {oxygenBehaviour.GetOxygen()} oxygen left and respawned at {playerPosition}");;
        
        
        Load();
    }
}
