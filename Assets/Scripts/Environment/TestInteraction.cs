using UnityEngine;

public class TestInteraction : MonoBehaviour, IInteractable
{
    private Outline m_Outline;

    private void Awake()
    {
        m_Outline = GetComponent<Outline>();
        m_Outline.enabled = false;
    }

    public bool destroyed { get; private set; }

    bool IInteractable.IsDestroyed => destroyed;
    
    public bool Select()
    {
        print("Interacted with TestInteraction");
        return false;
    }

    public void Deselect()
    {
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

