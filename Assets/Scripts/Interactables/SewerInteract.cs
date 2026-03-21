using UnityEngine;

public class SewerInteract : MonoBehaviour, IInteract {
    [SerializeField] private Vendor vendor = default;

    public GameObject GetNodeContainer() => null;

    public void Interact(GameObject interactor) {
        Debug.Log($"{gameObject.name}: {interactor.name} interact w/ {gameObject.name}");

        if (interactor.TryGetComponent<State>(out var interactorState)) {
            interactorState.SetState(State.AnimationName.Interacting);
        }

        vendor.Vend(interactor);
    }
}
