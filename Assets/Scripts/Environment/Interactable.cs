using UnityEngine;

public interface IInteractable
{
    void Interact(GameObject player);
    void Interact(GameObject player, GameObject item);
    void Interact();
    void StopInteract();
    void Highlight();
    void Unhighlight();
}
