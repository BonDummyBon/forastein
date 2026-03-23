using UnityEngine;

public class VendorInteract : MonoBehaviour, IInteract {
    [SerializeField] private Vendor vendor = default;
    [SerializeField] private GameObject nodeContainer = default;

    public event System.Action InteractionEnded;

    private void OnValidate() {
        if (vendor == null) {
            vendor = gameObject.TryGetComponent(out Vendor v) ? v : null;
        }
        if (nodeContainer == null) {
            nodeContainer = GameObject.Find("NodeContainer");
        }
    }

    public GameObject GetNodeContainer() {
        return nodeContainer;
    }

    public void Interact(GameObject interactor) {
        Debug.Log($"{gameObject.name}: {interactor.name} interact w/ {gameObject.name}");
        vendor.Vend(interactor, () => InteractionEnded?.Invoke());
    }
}
