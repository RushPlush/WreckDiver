using System;
using System.Linq;
using Inventory;
using UnityEditor.VersionControl;
using UnityEngine;

public class HarpoonBehaviour : MonoBehaviour, IItem {
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    bool isHarpoonActive = false;
    [SerializeField] private GameObject harpoon;
    [SerializeField] private Item harpoonItem;
    [SerializeField] private Item harpoonGunItem;
    private const int numberOfHarpoons = 2;
    [SerializeField] private Transform[] harpoonSpawnPoints;
    private GameObject[] harpoonInstances = new GameObject[numberOfHarpoons];
    private bool[] harpoonsReady = new bool[numberOfHarpoons];
    private Harpoon[] harpoonScripts = new Harpoon[numberOfHarpoons];
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
            {
                itemManager = foundObjects[i];
                //foundObjects[i].GetComponent<InventorySystem>().GetItem(harpoonGunItem).ItemBehaviour = this;
            }
        }
    }

    private void Start()
    {
        var harpoonsStored = itemManager.harpoonsLoaded; // seeing as it gets set in reload
        for (int i = 0; i < harpoonsStored; i++)
        {
            Reload();
        }
    }

    public void PrimaryUse() // Shoot
    {
        for (int i = 0; i < numberOfHarpoons; i++)
        {
            if (harpoonsReady[i])
            {
                itemManager.harpoonsLoaded--;
                harpoonScripts[i].Shoot();
                harpoonInstances[i].transform.SetParent(null, true);
                harpoonsReady[i] = false;
                return; // Only shoot one harpoon at a time
            }
            // todo what happens when it can't shoot
        }
    }

    public void SecondaryUse() // Pull?
    {
        throw new System.NotImplementedException();
    }

    public void TertiaryUse() //Reload
    {
        if (!itemManager.inventorySystem.HasItem(harpoonItem)) return; //todo what happens when it can't reload
        itemManager.inventorySystem.RemoveItem(harpoonItem);
        Reload();
    }

    private void Reload()
    {
        for (int i = 0; i < numberOfHarpoons; i++)
        {
            if(harpoonsReady[i] == false)
            {
                itemManager.harpoonsLoaded = i+1;
                harpoonInstances[i] = Instantiate(harpoon, harpoonSpawnPoints[i]);
                harpoonInstances[i].transform.SetParent(harpoonSpawnPoints[i], true);
                harpoonScripts[i] = harpoonInstances[i].GetComponent<Harpoon>();
                harpoonsReady[i] = true;
                return;
            }
            //todo what happens when it can't reload
        }
    }
}
