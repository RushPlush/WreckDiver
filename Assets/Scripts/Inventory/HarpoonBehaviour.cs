using System.Linq;
using Inventory;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.InputSystem;

public class HarpoonBehaviour : MonoBehaviour, IItem
{
    [SerializeField] private GameObject harpoon;
    [SerializeField] private Item harpoonItem;
    [SerializeField] private Item harpoonGunItem;
    private const int numberOfHarpoons = 2;
    [SerializeField] private Transform[] harpoonSpawnPoints;
    [SerializeField] private ParticleSystem[] harpoonParticles;
    private GameObject[] harpoonInstances = new GameObject[numberOfHarpoons];
    private bool[] harpoonsReady = new bool[numberOfHarpoons];
    private bool[] harpoonsExists = new bool[numberOfHarpoons];
    private Harpoon[] harpoonScripts = new Harpoon[numberOfHarpoons];
    private Pullable[] pullableScripts = new Pullable[numberOfHarpoons];
    [SerializeField] LineRenderer[] lineRenderers;
    private Pullable playerPullable;
    private ItemManager itemManager;
    int lastShotIndex = -1;
    bool skippedLastShot = false;
    private bool tethered = false;
    [SerializeField] private float maxTetherLength = 30f;

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
                harpoonParticles[i].Play();
                skippedLastShot = false;
                tethered = true;
                break;
            }
            // todo what happens when it can't shoot
        }
    }

    public void SecondaryUse(InputAction.CallbackContext context) // Pull?
    {
        if (context.started)
        {
            if (harpoonInstances[0] == null && harpoonInstances[1] == null) return; //todo what happens when it can't pull (when neither harpoon is instanced)

            //if (harpoonsReady[0] != false && harpoonsReady[1] != false && harpoonInstances[0] != null && harpoonInstances[1] != null)
            //{
            //    DoublePull();
            //}
            if (!harpoonsReady[lastShotIndex] && harpoonInstances[lastShotIndex] != null)
            {
                print("SinglePull");
                SinglePull();
            }
        }
        else if(context.canceled)
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
        if (harpoonInstances[lastShotIndex] == null) return; //todo what happens when it can't pull (when harpoon is not instanced)
        if (harpoonScripts[lastShotIndex] == null) return;
        if (!tethered) return;
        if (harpoonScripts[lastShotIndex].isHarpoonActive) return;
        if (pullableScripts[lastShotIndex] != null)
        {
            pullableScripts[lastShotIndex].Pull(playerPullable);
            playerPullable.Pull(pullableScripts[lastShotIndex]);
        }
        else
        {
            
            playerPullable.Pull(harpoonInstances[lastShotIndex].transform);
        }
        //for (int i = 0; i < numberOfHarpoons; i++)
        //{
        //    if (!harpoonsReady[i] && harpoonInstances[i] != null)
        //    {
        //        if (pullableScripts[i] != null)
        //        {
        //            pullableScripts[i].Pull(playerPullable);
        //            playerPullable.Pull(pullableScripts[i]);
        //        }
        //        else
        //        {
        //            if(!harpoonScripts[i].canPull) return;
        //            playerPullable.Pull(harpoonInstances[i].transform);
        //        }
        //    }
        //    // todo what happens when it can't pull
        //}
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
                harpoonsExists[i] = true;
                pullableScripts[i] = null;
                //lastShotIndex = -1; 
                return;
            }
            //todo what happens when it can't reload
            itemManager.inventorySystem.AddItem(harpoonItem, 1);
        }
    }

    public void SetPullable(int index, Pullable pullable)
    {
        pullableScripts[index] = pullable;
    }
    private void LateUpdate()
    {
        foreach (var lineRender in lineRenderers)
        {
            lineRender.SetPosition(0, lineRender.transform.position);
            lineRender.SetPosition(1, lineRender.transform.position);
        }
        if (!tethered) return;
        if(lastShotIndex == -1) return;
        if(harpoonsExists[lastShotIndex] == false) return;
        //lineRenderers[lastShotIndex].SetPosition(0, lineRenderers[lastShotIndex].transform.position);
        var connectionPoint = harpoonScripts[lastShotIndex].connectionPoint.transform.position;
        var distance = Vector3.Distance(connectionPoint, lineRenderers[lastShotIndex].transform.position);
        if(distance > maxTetherLength || !harpoonScripts[lastShotIndex].canPull)
        {
            tethered = false;
            //todo snap sound effect
        }
        else
        {
            lineRenderers[lastShotIndex].SetPosition(1, connectionPoint);
        }
    }
    public void DestroyHarpoon(int index)
    {
        harpoonsReady[index] = false;
        harpoonsExists[index] = false;
        harpoonScripts[index] = null;
        harpoonInstances[index] = null;
        lineRenderers[index].SetPosition(5, lineRenderers[index].GetPosition(4));
        pullableScripts[index] = null;
        playerPullable.Release();
    }
}
