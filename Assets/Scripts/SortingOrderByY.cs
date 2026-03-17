using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Renderer))]
public class SortingOrderByY : MonoBehaviour {
    #region Fields

    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private int orderPerUnit = 24;
    [SerializeField] private bool useFixedOrder;
    [SerializeField] private int fixedOrder;

    private Vector3 lastPos;
    private int lastOrder;

    #endregion

    #region Initialization

    private void Awake() {
        if (targetRenderer == null) {
            targetRenderer = GetComponent<Renderer>();
        }

        ApplyOrder(force: true);
    }

    #endregion

    #region Unity Callbacks

    private void LateUpdate() {
        ApplyOrder(force: false);
    }

    private void OnValidate() {
        if (orderPerUnit <= 0) {
            orderPerUnit = 1;
        }

        if (targetRenderer == null) {
            targetRenderer = GetComponent<Renderer>();
        }

        ApplyOrder(force: true);
    }

    #endregion

    #region Private Methods

    private void ApplyOrder(bool force) {
        if (targetRenderer == null) {
            return;
        }

        if (useFixedOrder) {
            if (force || lastOrder != fixedOrder) {
                targetRenderer.sortingOrder = fixedOrder;
                lastOrder = fixedOrder;
            }
            return;
        }

        Vector3 pos = transform.position;
        if (!force && pos == lastPos) {
            return;
        }

        int order = Mathf.RoundToInt(-pos.y * orderPerUnit);
        if (force || order != lastOrder) {
            targetRenderer.sortingOrder = order;
            lastOrder = order;
        }

        lastPos = pos;
    }

    #endregion
}
