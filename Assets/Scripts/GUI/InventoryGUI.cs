using UnityEngine;
using UnityEngine.UI;

public class InventoryGUI : MonoBehaviour {
    #region [Fields]

    [SerializeField] Inventory playerInventory = default;
    [SerializeField] private Image itemImage = default;
    [SerializeField] private Image itemFrame = default;

    private bool isInventoryEnabled = true;

    #endregion

    #region Initialization

    private void OnValidate() {
        if (playerInventory == null) {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) {
                if (player.TryGetComponent<Inventory>(out var inventory)) {
                    playerInventory = inventory;
                }
            }
        }
    }

    private void Start() {
        UpdateInventoryImage();
    }

    private void OnEnable() {
        playerInventory.OnInventoryChange += UpdateInventoryImage;
    }

    private void OnDisable() {
        playerInventory.OnInventoryChange -= UpdateInventoryImage;
    }

    #endregion

    #region Private Methods

    private void UpdateInventoryImage() {
        GameItem gameItem = playerInventory.GetFirstGameItem();
        Debug.Log($"{gameObject.name}: Updating Invetory Image w/ {(gameItem != null ? gameItem.name : null)}");
        if (gameItem == null) {
            TurnOffInventoryGUI();
        } else {
            if (!isInventoryEnabled) {
                TurnOnInventoryGUI();
            }

            Sprite gameItemSprite = gameItem.itemIcon;
            itemImage.sprite = gameItemSprite;
        }
    }

    private void TurnOffInventoryGUI() {
        itemImage.enabled = false;
        itemFrame.enabled = false;
        isInventoryEnabled = false;
        Debug.Log($"{gameObject.name}: Turned OFF Inventory.");
    }

    private void TurnOnInventoryGUI() {
        itemImage.enabled = true;
        itemFrame.enabled = true;
        isInventoryEnabled = true;
        Debug.Log($"{gameObject.name}: Turned ON Inventory.");
    }

    #endregion
}
