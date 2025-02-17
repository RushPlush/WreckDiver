using UnityEngine;

public class TestInteraction : MonoBehaviour, IInteractable
{
    public void Interact(GameObject player)
    {
        throw new System.NotImplementedException();
    }

    public void Interact(GameObject player, GameObject item)
    {
        throw new System.NotImplementedException();
    }

    public void Interact()
    {
        print("Interacted with TestInteraction");
    }

    public void StopInteract()
    {
        throw new System.NotImplementedException();
    }

    public void Highlight()
    {
        print(transform.name + " Highlighted");
    }

    public void Unhighlight()
    {
        throw new System.NotImplementedException();
    }
}

