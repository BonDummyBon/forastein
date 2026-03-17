using UnityEngine;

public class FollowPlayer : MonoBehaviour {
    #region Serialized Fields

    [SerializeField] private Transform playerTransform;
    [SerializeField] private float xMin;
    [SerializeField] private float xMax;
    [SerializeField] private float yMin;
    [SerializeField] private float yMax;

    #endregion

    #region Unity

    private void LateUpdate() {
        Vector3 playerPos = playerTransform.position;
        playerPos = new(Mathf.Clamp(playerPos.x, xMin, xMax), Mathf.Clamp(playerPos.y, yMin, yMax), -10);
        transform.position = playerPos;
    }

    #endregion
}
