using UnityEngine;

public class SewerInteract : MonoBehaviour, IInteract {
    [SerializeField] private Vendor vendor = default;

    public event System.Action InteractionEnded;

    public GameObject GetNodeContainer() => null;

    public void Interact(GameObject interactor) {
        Debug.Log($"{gameObject.name}: {interactor.name} interact w/ {gameObject.name}");

        if (interactor.TryGetComponent(out State interactorState)) {
            interactorState.SetState(State.AnimationName.Interacting);
        }

        vendor.Vend(interactor, () => InteractionEnded?.Invoke());
    }
}
