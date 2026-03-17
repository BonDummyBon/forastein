using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour {
    #region Enums

    public enum AnimationType { Soft, Hard }
    public enum AnimationName { Idle, DumpsterDiving, Moving, Positioning, SculptingDummy, Interacting }
    public enum AnimationSubN { None, Idle, Pathfinding, Positioning, Interacting }

    #endregion

    #region Fields

    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovement pMovement;
    [SerializeField] private PlayerInteraction pInteraction;
    [SerializeField] private bool verboseLogging = false;

    private AnimationType animType = AnimationType.Soft;
    private AnimationName animName = AnimationName.Idle;
    private AnimationSubN animSubN = AnimationSubN.Idle;

    #endregion

    #region Properties

    public AnimationType AnimType { get => animType; set { } }
    public AnimationName AnimName { get => animName; set { } }
    public AnimationSubN AnimSubN { get => animSubN; set { } }

    #endregion

    #region Initialization

    private void Awake() {
        pInteraction.Pathfinding += OnPathfinding;
    }

    #endregion

    #region Unity Callbacks

    private void Update() {
        if (verboseLogging) {
            Debug.Log($"{gameObject.name}: {animName} - {animSubN}");
            Debug.Log($"{gameObject.name}: isMoving: {animator.GetBool("isMoving")}, xVel: {animator.GetFloat("xVel")}, yVel: {animator.GetFloat("yVel")}");
        }
    }

    #endregion

    #region Public Methods

    public bool IsAnimCancellable() => animType != AnimationType.Hard;

    public void SetState(AnimationName stateName) {
        if (!IsAnimCancellable()) {
            return;
        }

        switch (stateName) {
            case AnimationName.Positioning:
                animType = AnimationType.Soft;
                animName = AnimationName.Positioning;
                animSubN = AnimationSubN.Positioning;
                break;
            case AnimationName.Idle:
                animType = AnimationType.Soft;
                animName = AnimationName.Idle;
                animSubN = AnimationSubN.Idle;
                break;
            case AnimationName.Moving:
                animType = AnimationType.Soft;
                animName = AnimationName.Moving;
                animSubN = AnimationSubN.Idle;
                break;
            case AnimationName.DumpsterDiving:
                animType = AnimationType.Hard;
                animName = AnimationName.DumpsterDiving;
                animSubN = AnimationSubN.Interacting;
                break;
            case AnimationName.SculptingDummy:
                animType = AnimationType.Hard;
                animName = AnimationName.SculptingDummy;
                animSubN = AnimationSubN.Interacting;
                break;
            case AnimationName.Interacting:
                animType = AnimationType.Hard;
                animName = AnimationName.Interacting;
                animSubN = AnimationSubN.Interacting;
                break;
        }
    }

    public void NextSubState() {
        if (animName == AnimationName.DumpsterDiving || animName == AnimationName.SculptingDummy || animName == AnimationName.Interacting) {
            animType = AnimationType.Soft;
            animName = AnimationName.Idle;
            animSubN = AnimationSubN.Idle;
        }
    }

    public void ChangeDirection(Vector2 dir) {
        animator.SetBool("isMoving", true);
        animator.SetFloat("xVel", dir.x);
        animator.SetFloat("yVel", dir.y);
    }

    #endregion

    #region Helpers

    private void OnPathfinding() => SetState(AnimationName.Positioning);

    #endregion
}
