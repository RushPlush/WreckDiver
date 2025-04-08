using System.Linq;
using Inventory;
using UnityEngine;
using UnityEngine.InputSystem;

public class HarpoonBehaviour : MonoBehaviour, IItem
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject harpoon;
    [SerializeField] private Item harpoonItem;
    [SerializeField] private Item harpoonGunItem;
    private const int numberOfHarpoons = 2;
    [SerializeField] private Transform[] harpoonSpawnPoints;
    private GameObject[] harpoonInstances = new GameObject[numberOfHarpoons];
    private bool[] harpoonsReady = new bool[numberOfHarpoons];
    private Harpoon[] harpoonScripts = new Harpoon[numberOfHarpoons];
    private Pullable[] pullableScripts = new Pullable[numberOfHarpoons];
    private Pullable playerPullable;
    private ItemManager itemManager;
    int lastShotIndex = -1;
    bool skippedLastShot = false;

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
                playerPullable = foundObjects[i].gameObject.GetComponent<Pullable>();
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

    bool previouseState = false;

    public void PrimaryUse(InputAction.CallbackContext context) // Shoot
    {
        if (!context.started) return;
        Shoot();
    }

    private void Shoot()
    {
        for (int i = 0; i < numberOfHarpoons; i++)
        {
            if (harpoonsReady[i])
            {
                itemManager.harpoonsLoaded--;
                harpoonScripts[i].Shoot();
                harpoonInstances[i].transform.SetParent(null, true);
                harpoonsReady[i] = false;
                lastShotIndex = i;
                skippedLastShot = false;
                return;
            }
            // todo what happens when it can't shoot
        }
    }

    public void SecondaryUse(InputAction.CallbackContext context) // Pull?
    {
        if (context.performed)
        {
            if (harpoonInstances[0] == null && harpoonInstances[1] == null) return; //todo what happens when it can't pull (when neither harpoon is instanced)

            if (harpoonsReady[0] != false && harpoonsReady[1] != false && harpoonInstances[0] != null && harpoonInstances[1] != null)
            {
                DoublePull();
            }
            else if (!harpoonsReady[1] && harpoonInstances[1] != null ||
                     !harpoonsReady[0] && harpoonInstances[0] != null)
            {
                print("SinglePull");
                SinglePull();
            }
        }
        else
        {
            StopPull();
        }
    }

    void StopPull()
    {
        for (int i = 0; i < numberOfHarpoons; i++)
        {
            if (pullableScripts[i] != null)
            {
                pullableScripts[i].Stop();
            }
            playerPullable.Stop();
        }
    }

    public void SinglePull()
    {
        for (int i = 0; i < numberOfHarpoons; i++)
        {
            if (!harpoonsReady[i] && harpoonInstances[i] != null)
            {
                if (pullableScripts[i] != null)
                {
                    pullableScripts[i].Pull(playerPullable);
                    playerPullable.Pull(pullableScripts[i]);
                }
                else
                {
                    playerPullable.Pull(harpoonInstances[i].transform);
                }
            }
            // todo what happens when it can't pull
        }
    }

    void DoublePull()
    {
        playerPullable.Release();
        if (pullableScripts[0] == null && pullableScripts[1] == null) return; // does nothing, can't pull anything
        if (pullableScripts[0] == null)
        {
            pullableScripts[1].Pull(harpoonInstances[0].transform);
            return;
        }
        else if (pullableScripts[1] == null)
        {
            pullableScripts[0].Pull(harpoonInstances[1].transform);
            return;
        }
        pullableScripts[0].Pull(pullableScripts[1]);
        pullableScripts[1].Pull(pullableScripts[0]);
    }

    public void TertiaryUse(InputAction.CallbackContext context) //Reload
    {
        if (!itemManager.inventorySystem.HasItem(harpoonItem) || !context.started) return; //todo what happens when it can't reload
        itemManager.inventorySystem.RemoveItem(harpoonItem);
        Reload();
    }

    private void Reload()
    {
        for (int i = 0; i < numberOfHarpoons; i++)
        {
            if (!harpoonsReady[0] && !harpoonsReady[1])
            {
                if (i == lastShotIndex && !skippedLastShot)
                {
                    skippedLastShot = true;
                    continue;
                }
            }
            if (harpoonsReady[i] == false)
            {
                itemManager.harpoonsLoaded = i + 1;
                harpoonInstances[i] = Instantiate(harpoon, harpoonSpawnPoints[i]);
                harpoonInstances[i].transform.SetParent(harpoonSpawnPoints[i], true);
                harpoonScripts[i] = harpoonInstances[i].GetComponent<Harpoon>();
                harpoonScripts[i].SetIndexValue(i);
                harpoonScripts[i].SetHarpoonBehaviour(this);
                harpoonsReady[i] = true;
                pullableScripts[i] = null;
                return;
            }
            //todo what happens when it can't reload
        }
    }

    public void SetPullable(int index, Pullable pullable)
    {
        pullableScripts[index] = pullable;
    }

    public void DestroyHarpoon(int index)
    {
        harpoonsReady[index] = false;
        harpoonScripts[index] = null;
        harpoonInstances[index] = null;
        pullableScripts[index] = null;
        playerPullable.Release();
    }
}
