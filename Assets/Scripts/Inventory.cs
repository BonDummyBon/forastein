using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Inventory : MonoBehaviour {


    #region Fields

    private readonly List<GameItem> items = new();

    [SerializeField]
    private int size = default;
    public int Size => size;

    #endregion

    #region Events

    public event Action OnInventoryChange;

    #endregion


    #region Public Methods

    public GameItem GetFirstGameItem() {
        if (items.Count == 0) {
            return null;
        }

        return items[0];
    }

    public IReadOnlyList<GameItem> GetAllGameItems() {
        return items;
    }

    public bool AddGameItem(GameItem gameItem) {
        if (items.Count + 1 > Size) {
            return false;
        }

        items.Add(gameItem);
        OnInventoryChange?.Invoke();
        return true;
    }

    public bool AddGameItem(VendingRecipeElement recipeOutput) {
        if (items.Count + recipeOutput.Count > Size) {
            return false;
        }

        for (int i = 0; i < recipeOutput.Count; i++) {
            items.Add(recipeOutput.Item);
        }

        OnInventoryChange?.Invoke();
        return true;
    }

    public GameItem RemoveGameItem() {
        if (items.Count == 0) {
            return null;
        }

        GameItem removedItem = items[0];
        items.RemoveAt(0);
        OnInventoryChange?.Invoke();
        return removedItem;
    }

    public GameItem RemoveGameItem(GameItem gameItem) {
        bool wasItemRemoved = items.Remove(gameItem);
        if (wasItemRemoved) {
            OnInventoryChange?.Invoke();
        }
        return wasItemRemoved ? gameItem : null;
    }

    public bool RemoveGameItem(VendingRecipeElement recipeInput) {
        int count = items.Count(x => x == recipeInput.Item);
        if (count < recipeInput.Count) {
            return false;
        }

        int removed = 0;
        items.RemoveAll(x => {
            if (x == recipeInput.Item && removed < recipeInput.Count) {
                removed++;
                return true;
            }
            return false;
        });

        OnInventoryChange?.Invoke();
        return true;
    }

    public bool IsFull() {
        return items.Count == Size;
    }

    public int GetItemCount() {
        return items.Count;
    }

    #endregion

    private void Update() {
        Debug.Log($"{gameObject.name}: Inventory: {string.Join(", ", items)}");
    }
}
