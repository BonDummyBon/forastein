using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding {
    #region Fields

    public static bool verboseLogging = true;
    private static readonly int defaultLayerMask = LayerMask.GetMask("Default");

    #endregion

    #region Public Methods

    public static List<Vector2> Pathfind(GameObject finderGO, GameObject findeeGO) {
        Transform nodecontainerTransform = findeeGO.transform.Find("NodeContainer");
        if (nodecontainerTransform == null) {
            if (verboseLogging) {
                Debug.LogWarning($"No NodeContainer found on {findeeGO.name}");
            }

            return new List<Vector2>();
        }

        GameObject nodeContainer = nodecontainerTransform.gameObject;
        PathNode closestNode = FindClosestNode(finderGO, nodeContainer);

        if (verboseLogging) {
            Debug.Log($"Closest node to {finderGO.name} is {closestNode?.gameObject.name}");
        }

        List<Vector2> pathList = FindPath(closestNode);

        if (verboseLogging) {
            Debug.Log($"Path found: {string.Join(" -> ", pathList)}");
        }

        return pathList;
    }

    #endregion

    #region Private Methods

    private static PathNode FindClosestNode(GameObject finderGO, GameObject nodeContainer) {
        for (int i = 0; i < nodeContainer.transform.childCount; i++) {
            GameObject node = nodeContainer.transform.GetChild(i).gameObject;

            Vector2 origin = (Vector2)finderGO.transform.position;
            Vector2 target = (Vector2)node.transform.position;
            float dist = Vector2.Distance(origin, target);
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, target - origin, dist, defaultLayerMask);

            if (verboseLogging) {
                foreach (RaycastHit2D hit in hits) {
                    Debug.Log($"  Hit: {hit.collider.gameObject.name} at {hit.point}");
                }
            }

            if (hits.Length == 1) {
                return node.GetComponent<PathNode>();
            }
        }

        return null;
    }

    private static List<Vector2> FindPath(PathNode startNode) {
        List<Vector2> path = new();
        PathNode curNode = startNode;

        do {
            if (verboseLogging) {
                Debug.Log($"Current node: {curNode?.gameObject.name} | Connections: {curNode.Next.Count}");
            }

            path.Add(curNode.Pos);

            if (curNode.Next.Count == 0) {
                break;
            }

            if (verboseLogging) {
                Debug.Log($"Next node: {curNode.Next[0]?.node?.gameObject.name}");
            }

            curNode = curNode.Next[0].node;

            if (verboseLogging) {
                Debug.Log($"Moved to node: {curNode?.gameObject.name}");
            }
        }
        while (curNode != null);

        return path;
    }

    #endregion
}
