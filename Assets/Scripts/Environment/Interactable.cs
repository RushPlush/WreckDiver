using UnityEngine;

public interface IInteractable
{
    enum InteractionType
    {
        [Tooltip("Interact with an item, sends the item to the interactable, usually needing specific items")]
        ItemInteraction,
        [Tooltip("Interact with the player, usually needing the player to be in a specific state, or affecting the player in some way")]
        PlayerInteraction,
        [Tooltip("Interact with both the player and an item, usually needing the player to be in a specific state, or affecting the player in some way, and or needing specific items")]
        ItemAndPlayerInteraction,
        [Tooltip("Doesn't need any specific items or player state")]
        FreeInteraction,
        [Tooltip("Can't interact with this object, usually used for items that need some perquisites to be interacted with, and then needs to get changed to one of the other types")]
        NoInteraction
    }
    bool destroyed { get; protected set; }
    public InteractionType interactionType { get; protected set; }
    void Interact(GameObject player);
    void Interact(GameObject player, Item item);
    void Interact(Item item);
    void Interact();
    void StopInteract();
    void InteractionFailed();
    void Highlight();
    void Unhighlight();
}
