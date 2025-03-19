using System;
using System.Linq;
using Inventory;
using UnityEngine;

public class HarpoonBehaviour : MonoBehaviour, IItem {
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    bool isHarpoonActive = false;
    [SerializeField] private GameObject harpoon;
    [SerializeField] private Item harpoonItem;
    private ItemManager itemManager;

    private void Awake()
    {
        // in scene, find player by tag, get item manager
        var foundObjects = FindObjectsByType<ItemManager>(FindObjectsSortMode.None);
        if (foundObjects.Length == 0)
        {
            Debug.LogError("No ItemManager found in scene");
            return;
        }
        for (int i = 0; i < foundObjects.Count(); i++)
        {
            if (foundObjects[i].gameObject.CompareTag("Player"))
                itemManager = foundObjects[i];
        }
    }

    public void PrimaryUse() // Shoot
    {
        
        throw new System.NotImplementedException();
    }

    public void SecondaryUse() // Pull?
    {
        throw new System.NotImplementedException();
    }

    public void TertiaryUse() //Reload
    {
        throw new System.NotImplementedException();
    }
}
