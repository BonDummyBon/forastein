using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour {

    #region Fields

    private Vector2 inputAxis = default;
    public static PlayerInput Instance;
    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        } else {
            Instance = this;
        }
    }

    #endregion

    #region Properties

    public Vector2 InputAxis { get => inputAxis; set { } }

    #endregion

    #region Events

    public event System.Action InteractionKeyPressed;

    #endregion

    #region Unity Methods

    private void Update() {
        UpdateInputAxis();
        CheckInteractionKey();
    }

    #endregion

    #region Input Handling

    private void UpdateInputAxis() {
        Vector2 temp = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) {
            temp.y += 1f;
        }

        if (Keyboard.current.sKey.isPressed) {
            temp.y -= 1f;
        }

        if (Keyboard.current.dKey.isPressed) {
            temp.x += 1f;
        }

        if (Keyboard.current.aKey.isPressed) {
            temp.x -= 1f;
        }

        inputAxis = temp.normalized;
    }

    private void CheckInteractionKey() {
        if (Keyboard.current != null) {
            if (Keyboard.current.eKey.wasPressedThisFrame) {
                InteractionKeyPressed?.Invoke();
            }
        }
    }

    #endregion
}
