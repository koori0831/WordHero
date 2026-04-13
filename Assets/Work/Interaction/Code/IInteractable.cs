using UnityEngine;

public interface IInteractable
{
    bool CanInteract { get; }
    void Interact(GameObject interactor);
}
