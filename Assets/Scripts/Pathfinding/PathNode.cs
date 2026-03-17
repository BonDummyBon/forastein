using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PathRef {
    public PathNode node;
    [HideInInspector] public float dist;
}

public class PathNode : MonoBehaviour {
    #region Fields

    [SerializeField] private List<PathRef> next = new();
    [SerializeField] private List<PathRef> prev = new();

    #endregion

    #region Properties

    public Vector2 Pos => gameObject.transform.position;
    public IReadOnlyList<PathRef> Next => next;
    public IReadOnlyList<PathRef> Prev => prev;

    #endregion

    #region Unity Callbacks

    private void Start() {
        CalcDists();
    }

    #endregion

    #region Private Methods

    private void CalcDists() {
        for (int i = 0; i < next.Count; i++) {
            if (next[i]?.node != null) {
                next[i].dist = Vector2.Distance(next[i].node.Pos, Pos);
            }
        }

        for (int i = 0; i < prev.Count; i++) {
            if (prev[i]?.node != null) {
                prev[i].dist = Vector2.Distance(prev[i].node.Pos, Pos);
            }
        }
    }

    #endregion
}
