using UnityEngine;

public interface IHighlightable
{
    void Highlight();
    void Unhighlight();
}

public interface IInteractable : IHighlightable
{
    bool IsDestroyed { get; }
    bool Select();
    void Deselect();
}

public interface IInteractableWithPlayer : IInteractable
{
    bool Select(GameObject player);
    void Deselect(GameObject player);
}

public interface IInteractableWithItem : IInteractable
{
    bool Select(Item item);
    void Deselect(Item item);
}

public interface IInteractableWithPlayerAndItem : IInteractableWithPlayer, IInteractableWithItem
{
    bool Select(GameObject player, Item item);
    void Deselect(GameObject player, Item item);
}