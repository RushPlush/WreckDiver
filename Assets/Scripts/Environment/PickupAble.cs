using UnityEngine;

public class PickupAble : MonoBehaviour, IInteractable
{
    private IInteractable.InteractionType interactionType1 = IInteractable.InteractionType.PlayerInteraction;
    [SerializeField] private Item item;
    [SerializeField] private int quantity = 1;
    private Outline m_Outline;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        m_Outline = GetComponent<Outline>();
        m_Outline.enabled = false;
    }
    
    IInteractable.InteractionType IInteractable.interactionType
    {
        get => interactionType1;
        set => interactionType1 = value;
    }

    public void Interact(GameObject player)
    {
        print(gameObject.name + " Interacted with player");
        player.GetComponent<ItemManager>().inventorySystem.AddItem(item, quantity);
        Destroy(this.gameObject);
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
        throw new System.NotImplementedException();
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
        m_Outline.enabled = true;
    }

    public void Unhighlight()
    {
        m_Outline.enabled = false;
    }
}
