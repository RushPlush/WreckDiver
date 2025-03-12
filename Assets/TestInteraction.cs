using UnityEngine;

public class TestInteraction : MonoBehaviour, IInteractable
{
    private Outline m_Outline;
    [SerializeField]private IInteractable.InteractionType interactionType;
    
    private void Awake()
    {
        m_Outline = GetComponent<Outline>();
        m_Outline.enabled = false;
    }

    IInteractable.InteractionType IInteractable.interactionType
    {
        get => interactionType;
        set => interactionType = value;
    }

    public void Interact(GameObject player)
    {
        throw new System.NotImplementedException();
    }

    public void Interact(GameObject player, Item item)
    {
        throw new System.NotImplementedException();
    }

    public void Interact(Item item)
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

    public void InteractionFailed()
    {
        throw new System.NotImplementedException();
    }

    public void Highlight()
    {
        print(transform.name + " Highlighted");
        m_Outline.enabled = true;
    }

    public void Unhighlight()
    {
        m_Outline.enabled = false;
    }
}

