using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class OxygenPocketBehaviour : MonoBehaviour
{
    // TODO: Maybe have a set of oxygen available before pocket is empty

    void Start()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        var oxygenBehaviour = other.gameObject.GetComponent<OxygenBehaviour>();
        if (oxygenBehaviour != null)
        {
            oxygenBehaviour.InOxygenPocket = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        var oxygenBehaviour = other.gameObject.GetComponent<OxygenBehaviour>();
        if (oxygenBehaviour != null)
        {
            oxygenBehaviour.InOxygenPocket = false;
        }
        var SaveAndLoad = other.gameObject.GetComponent<SaveAndLoad>();
        if (SaveAndLoad != null)
        {
            SaveAndLoad.Save();
        }
    }
}
