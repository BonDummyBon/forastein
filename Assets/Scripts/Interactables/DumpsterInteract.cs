using UnityEngine;

public class DumpsterInteract : MonoBehaviour, IInteract {
    #region Fields

    [SerializeField] private GameObject nodeContainer = default;
    [SerializeField] private Vendor gameItemVendor = default;

    #endregion

    #region Public Methods

    public GameObject GetNodeContainer() => nodeContainer;

    // Interacts --> Tries to give them a Mud item.
    public void Interact(GameObject interactor) {
        Debug.Log($"{gameObject.name}: {interactor.name} interact w/ {gameObject.name}");

        if (interactor.TryGetComponent<State>(out var interactorState)) {
            interactorState.SetState(State.AnimationName.Interacting);
        }

        // Stop interaction if they already have an item in their inventory. We don't want them to be able to dumpster dive for infinite items.
        if (interactor.TryGetComponent<Inventory>(out var interactorInventory)) {
            GameItem item = interactorInventory.GetFirstGameItem();
            if (item != null) {
                return;
            }
        }

        gameItemVendor.Vend(interactor);
    }

    #endregion
}
