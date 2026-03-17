using UnityEngine;

public interface IInteract {
    public GameObject GetNodeContainer();
    public void Interact(GameObject interactor) {
        if (interactor == null) {
            return;
        }
    }
}
