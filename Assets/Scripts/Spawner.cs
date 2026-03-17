using UnityEngine;
using System;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {
    [SerializeField]
    private List<SpawnerEntry> entries = new();

    public IReadOnlyList<SpawnerEntry> Entries => entries;

    [SerializeField] private Inventory inventory = default;

    private void Awake() {
        BuildTimerDictionary();
    }

    private void BuildTimerDictionary() {
        foreach (SpawnerEntry entry in entries) {
            entry.timer = 0f;
        }
    }

    private void Update() {
        Debug.Log($"{gameObject.name} is updating spawner entries");
        foreach (SpawnerEntry entry in entries) {
            if (entry.time <= 0f) {
                continue;
            }

            if (inventory.IsFull()) {
                continue;
            }

            entry.timer += Time.deltaTime;

            if (entry.timer >= entry.time) {
                entry.timer -= entry.time;
                Debug.Log($"{gameObject.name} spawned {entry.GameItem.name}");
                SpawnGameItem(entry.GameItem);
            }
        }
    }

    private void SpawnGameItem(GameItem gameItem) {
        if (inventory != null) {
            inventory.AddGameItem(gameItem);
        }
    }
}
