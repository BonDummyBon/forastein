using UnityEngine;

public class TableInteract : MonoBehaviour, IInteract {

    [SerializeField] private Vendor gameItemVendor = default;

    public GameObject GetNodeContainer() => null;

    public void Interact(GameObject interactor) {
        Debug.Log($"{gameObject.name}: {interactor.name} interact w/ {gameObject.name}");

        if (interactor.TryGetComponent<State>(out var interactorState)) {
            interactorState.SetState(State.AnimationName.Interacting);
        }

        gameItemVendor.Vend(interactor);
    }
}
