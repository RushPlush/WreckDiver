using UnityEngine;

public interface IHighlightable
{
    void Highlight();
    void Unhighlight();
}

public interface IInteractable : IHighlightable
{
    bool IsDestroyed { get; }
    void Select();
    void Deselect();
}

public interface IInteractableWithPlayer : IInteractable
{
    void Select(GameObject player);
    void Deselect(GameObject player);
}

public interface IInteractableWithItem : IInteractable
{
    void Select(Item item);
    void Deselect(Item item);
}

public interface IInteractableWithPlayerAndItem : IInteractableWithPlayer, IInteractableWithItem
{
    void Select(GameObject player, Item item);
    void Deselect(GameObject player, Item item);
}