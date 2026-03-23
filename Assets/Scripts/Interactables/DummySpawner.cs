using System;
using UnityEngine;

public class DummySpawner : MonoBehaviour {
    [SerializeField] private DummyInteract dummyInteract = default;
    [SerializeField] private Inventory inventory = default;

    private void CheckIfCreatureMade() {
        if (inventory == null) {
            return;
        }

        GameItem item = inventory.GetFirstGameItem();
        if (item == null) {
            return;
        }

        switch (item.name) {
            case "Mud Monster":
                Debug.Log($"DummySpawner: {item.name} was made");
                break;
        }
    }

    private void OnEnable() {
        if (dummyInteract != null) {
            dummyInteract.InteractionEnded += CheckIfCreatureMade;
        }
    }

    private void OnDisable() {
        if (dummyInteract != null) {
            dummyInteract.InteractionEnded -= CheckIfCreatureMade;
        }
    }


}
