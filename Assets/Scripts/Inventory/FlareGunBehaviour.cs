using System.Linq;
using Inventory;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class FlareGunBehaviour : MonoBehaviour, IItem
{
    private ItemManager itemManager;
    [SerializeField] private ParticleSystem flareParticles;
    [SerializeField] private Item flareItem;
    private bool reloaded = false;
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
    public void PrimaryUse(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if(!reloaded) return;
        flareParticles.Play();
        reloaded = false;
        itemManager.inventorySystem.RemoveItem(flareItem); //incase player swaps items without firing first, it stops item from being used regardless
    }

    public void SecondaryUse(InputAction.CallbackContext context)
    {
        // unused
    }

    public void TertiaryUse(InputAction.CallbackContext context)
    {
        if (!itemManager.inventorySystem.HasItem(flareItem) || !context.started) return; //todo what happens when it can't reload
        if (reloaded) return;
        Reload();
    }
    public void Reload()
    {
        reloaded = true;
    }
}
