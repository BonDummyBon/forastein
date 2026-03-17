using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    #region Fields

    [SerializeField] private State pState;
    [SerializeField] private PlayerInteraction pInteraction;

    [SerializeField] private float moveSpeed = default;
    [SerializeField] private Rigidbody2D rb2D;
    [SerializeField] private SpriteRenderer spriteRen;
    [SerializeField] private Animator animator;

    private Vector2 moveAxis = default;
    private List<Vector2> path = new();

    private bool isPositioning = false;

    #endregion

    #region Properties

    public Vector2 MoveAxis { get => moveAxis; set { } }

    private Vector2 InputAxis => PlayerInput.Instance.InputAxis;

    #endregion

    #region Events

    public event System.Action FinishedPositioning;

    #endregion

    #region Unity Methods

    private void Awake() {
        pInteraction.Pathfinding += OnPathfinding;
    }

    private void Update() {
        TryMoving();
    }

    private void FixedUpdate() {
        switch (pState.AnimName) {
            case State.AnimationName.Moving:
                Moving();
                break;
            case State.AnimationName.Positioning:
                Positioning();
                break;
            default:
                moveAxis = Vector2.zero;
                break;
        }

        rb2D.linearVelocity = moveAxis;
        if (pState.IsAnimCancellable()) {
            MovementAnimatorVars();
        }
    }

    #endregion

    #region Movement

    private void Moving() {
        moveAxis = InputAxis * moveSpeed;
    }

    private void Positioning() {
        if (path.Count > 0 && Vector2.Distance((Vector2)transform.position, path[0]) < 0.1f) {
            path.RemoveAt(0);
        }

        if (path.Count == 0 && isPositioning) {
            FinishedPositioning?.Invoke();
            isPositioning = false;
        } else {
            Vector2 dir = (path[0] - (Vector2)transform.position).normalized;
            moveAxis = 1.2f * moveSpeed * dir;
        }
    }

    #endregion

    #region Input

    private void TryMoving() {
        if (InputAxis != Vector2.zero && pState.IsAnimCancellable()) {
            pState.SetState(State.AnimationName.Moving);
            path.Clear();
        }
    }

    #endregion

    #region Public Methods

    public void ChangeDirection(Vector2 newDir) {
        moveAxis = newDir.normalized;
    }

    #endregion

    #region Helpers

    private void OnPathfinding() {
        Debug.Log($"{gameObject.name}: Player started pathfinding interaction!");
        path = Pathfinding.Pathfind(gameObject, pInteraction.ActiveInteractable);
        isPositioning = true;
    }

    private void MovementAnimatorVars() {
        Vector2 temp = new(MoveAxis.x == 0 ? 0 : MoveAxis.x > 0 ? 1 : -1, MoveAxis.y == 0 ? 0 : MoveAxis.y > 0 ? 1 : -1);

        animator.SetBool("isMoving", Mathf.Abs(temp.magnitude) > 0);
        animator.SetFloat("xVel", temp.x);
        animator.SetFloat("yVel", temp.y);
    }

    #endregion
}
