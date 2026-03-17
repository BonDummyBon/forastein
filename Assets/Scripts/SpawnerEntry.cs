using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnerEntry {
    [SerializeField] private List<GameItem> gameItems;
    public GameItem GameItem {
        get {
            if (gameItems.Count == 0) {
                return null;
            }

            GameItem gameItem = gameItems[UnityEngine.Random.Range(0, gameItems.Count)];
            Debug.Log($"Spawned: {gameItem}");
            return gameItem;
        }
    }
    public float time;

    [NonSerialized]
    public float timer = 0f;
}
