using UnityEngine;

public class PixelPerfectPositioning : MonoBehaviour {
    public float PixelsPerUnit = 24f;

    private void OnValidate() {
        UpdatePixelsPerUnit();
        UpdatePosition();
    }

    private void UpdatePixelsPerUnit() {
        foreach (PixelPerfectPositioning comp in FindObjectsByType<PixelPerfectPositioning>(FindObjectsSortMode.None)) {
            comp.PixelsPerUnit = this.PixelsPerUnit;
        }
    }

    private void UpdatePosition() {
        Vector3 position = transform.position;
        position.x = Mathf.Round(position.x * PixelsPerUnit) / PixelsPerUnit;
        position.y = Mathf.Round(position.y * PixelsPerUnit) / PixelsPerUnit;
        transform.position = position;
    }
}
