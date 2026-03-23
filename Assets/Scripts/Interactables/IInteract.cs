using UnityEngine;

public interface IInteract {
    public GameObject GetNodeContainer();

    public event System.Action InteractionEnded;
    public void Interact(GameObject interactor) {
        if (interactor == null) {
            return;
        }
    }
}
