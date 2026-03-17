using UnityEngine;

public class PlayerInteraction : MonoBehaviour {
    #region Fields

    [SerializeField] private GameObject interactionFOV;
    [SerializeField] private TriggerSenser triggerSensor;
    [SerializeField] private PlayerMovement pMovement;
    [SerializeField] private State pState;


    private GameObject closestInteractable; //The interactable the player is currently looking at
    private GameObject activeInteractable; //  The interactable the player is currently interacting with

    #endregion

    #region Properties

    public GameObject ActiveInteractable { get => activeInteractable; set { } }

    #endregion

    #region Events

    public event System.Action Pathfinding;

    #endregion

    #region Unity Methods

    private void Awake() {
        pMovement.FinishedPositioning += OnFinishedPositioning;
        PlayerInput.Instance.InteractionKeyPressed += TryInteraction;
    }

    private void OnDestroy() {
        pMovement.FinishedPositioning -= OnFinishedPositioning;
        PlayerInput.Instance.InteractionKeyPressed -= TryInteraction;
    }

    private void Update() {
        UpdateFOVDirection();
        RefreshNearestInteractable();
    }

    #endregion

    #region Interaction

    private void TryInteraction() {
        if (closestInteractable == null || !pState.IsAnimCancellable()) {
            return;
        }

        if (!closestInteractable.TryGetComponent<IInteract>(out var interact)) {
            return;
        }

        activeInteractable = closestInteractable;

        Debug.Log($"{gameObject.name}: Is node container null? {interact.GetNodeContainer() == null}");
        if (interact.GetNodeContainer() != null) {
            Pathfinding?.Invoke();
        } else {
            ExecuteInteraction();
        }
    }

    private void ExecuteInteraction() {
        IInteract curInteract = activeInteractable.GetComponent<IInteract>();
        Debug.Log($"{gameObject.name}: Executing interaction with " + activeInteractable.name + " " + curInteract);
        curInteract?.Interact(gameObject);
    }

    private void OnFinishedPositioning() {
        ExecuteInteraction();
        FaceActiveInteractable();
    }

    #endregion

    #region Helpers

    private void UpdateFOVDirection() {
        Vector2 axis = pMovement.MoveAxis;
        if (axis == Vector2.zero) {
            return;
        }

        float angle = Mathf.Atan2(axis.y, axis.x) * Mathf.Rad2Deg;
        interactionFOV.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void RefreshNearestInteractable() {
        closestInteractable = GetClosestInteractable();
    }

    private void FaceActiveInteractable() {
        if (activeInteractable == null) {
            return;
        }

        Vector2 dir = (activeInteractable.transform.position - transform.position).normalized;
        pMovement.ChangeDirection(dir);
        pState.ChangeDirection(dir);
    }

    private GameObject GetClosestInteractable() {
        GameObject closest = null;
        float closestDist = Mathf.Infinity;

        foreach (var go in triggerSensor.Current) {
            if (go == null || !go.CompareTag("Interactable")) {
                continue;
            }

            float dist = (go.transform.position - transform.position).sqrMagnitude;
            if (dist < closestDist) {
                closestDist = dist;
                closest = go;
            }
        }
        return closest;
    }
    #endregion
}
