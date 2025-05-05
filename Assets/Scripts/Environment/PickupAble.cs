using System;
using UnityEngine;

public class PickupAble : MonoBehaviour, IInteractableWithPlayer
{
    [SerializeField] private Item item;
    [SerializeField] private int quantity = 1;
    private Outline m_Outline;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        m_Outline = GetComponent<Outline>();
        m_Outline.enabled = false;
    }

    public bool destroyed { get; private set; }

    bool IInteractable.IsDestroyed => destroyed;
    public bool Select()
    {
        return false;
    }

    public void Deselect()
    {
        throw new NotImplementedException();
    }

    private void OnDestroy()
    {
        destroyed = true;
    }

    public bool Select(GameObject player)
    {
        print(gameObject.name + " Interacted with player");
        player.GetComponent<ItemManager>().inventorySystem.AddItem(item, quantity);
        Destroy(this.gameObject);
        return false;
    }

    public void Deselect(GameObject player)
    {
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
