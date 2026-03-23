using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vendor : MonoBehaviour {
    [SerializeField] private Inventory inventory = default;
    [SerializeField] private VendingRecipe[] recipeList;

    public void Vend(GameObject interactor) {
        Vend(interactor, null);
    }

    public void Vend(GameObject interactor, System.Action interactionEnded) {
        StartCoroutine(DelayedVending(interactor, interactionEnded));
    }

    private IEnumerator DelayedVending(GameObject interactor, System.Action interactionEnded) {
        VendingRecipe recipe = RecipeFinder.Find(interactor, gameObject, recipeList);

        if (recipe != null) {
            Debug.Log($"{gameObject.name}: When {interactor.name} interacted with {gameObject.name}, it produced {recipe}");

            yield return recipe.CachedWaitTime;

            if (interactor.TryGetComponent(out Inventory interactorInventory)) {
                List<GameItem> interactorItems = new();
                List<GameItem> vendorItems = new();
                RemoveItems(interactorInventory, interactorItems, inventory, vendorItems, recipe);
                AddItems(interactorItems, interactorInventory, vendorItems, inventory, recipe);

            }
        }

        if (interactor.TryGetComponent(out State interactorState)) {
            interactorState.NextSubState();
        }

        interactionEnded?.Invoke();
    }

    private static void AddItems(List<GameItem> interactorItems, Inventory interactorInventory, List<GameItem> vendorItems, Inventory vendorInventory, VendingRecipe recipe) {
        foreach (VendingRecipeElement element in recipe.Outputs) {
            switch (element.Source) {
                case VendingRecipeElement.ItemSource.Fixed: {
                        switch (element.Port) {
                            case VendingRecipeElement.ItemPort.Interactor:
                                interactorInventory.AddGameItem(element);
                                interactorItems.Remove(element.Item);
                                break;
                            case VendingRecipeElement.ItemPort.Vendor:
                                vendorInventory.AddGameItem(element);
                                vendorItems.Remove(element.Item);
                                break;
                            case VendingRecipeElement.ItemPort.Either:
                                for (int i = 0; i < element.Count; i++) {
                                    if (!interactorInventory.AddGameItem(element.Item)) {
                                        if (!vendorInventory.AddGameItem(element.Item)) {
                                            Debug.LogError($"Recipe adding failed: inventories are full.");
                                        } else {
                                            vendorItems.Remove(element.Item);
                                        }
                                    } else {
                                        interactorItems.Remove(element.Item);
                                    }
                                }

                                break;
                        }
                        break;
                    }
                case VendingRecipeElement.ItemSource.PortFirstItem: {
                        switch (element.Port) {
                            case VendingRecipeElement.ItemPort.Interactor:
                                interactorInventory.AddGameItem(vendorItems[0]);
                                vendorItems.RemoveAt(0);
                                break;
                            case VendingRecipeElement.ItemPort.Vendor:
                                vendorInventory.AddGameItem(interactorItems[0]);
                                interactorItems.RemoveAt(0);
                                break;
                            case VendingRecipeElement.ItemPort.Either:
                                Debug.LogError($"Recipe adding failed: Prohibited settings.");
                                break;
                        }
                        break;
                    }
            }
        }
    }

    private static void RemoveItems(
        Inventory interactorInventory,
        List<GameItem> interactorItems,
        Inventory vendorInventory,
        List<GameItem> vendorItems,
        VendingRecipe recipe
    ) {

        RemoveItems(recipe.InteractorInputs, interactorInventory, interactorItems);
        RemoveItems(recipe.VendorInputs, vendorInventory, vendorItems);
        RemoveItems(recipe.EitherInputs, interactorInventory, vendorInventory, interactorItems, vendorItems);
    }

    private static void RemoveItems(
        List<VendingRecipeElement> recipeElements,
        Inventory inventory,
        List<GameItem> items) {
        foreach (VendingRecipeElement element in recipeElements) {
            switch (element.Source) {
                case VendingRecipeElement.ItemSource.Fixed:
                    for (int i = 0; i < element.Count; i++) {
                        GameItem removedItem = inventory.RemoveGameItem(element.Item);
                        if (removedItem == null) {
                            //Add inventory resetter to fix.
                        }
                        items.Add(removedItem);
                    }
                    break;
                case VendingRecipeElement.ItemSource.PortFirstItem:
                    for (int i = 0; i < element.Count; i++) {
                        GameItem removedItem = inventory.RemoveGameItem();
                        if (removedItem == null) {
                            //Add inventory resetter to fix.   }
                        }
                        items.Add(removedItem);
                    }
                    break;
            }
        }
    }
    private static void RemoveItems(
        List<VendingRecipeElement> recipeElements,
        Inventory interactorInventory,
        Inventory vendorInventory,
        List<GameItem> interactorItems,
        List<GameItem> vendorItems) {
        foreach (VendingRecipeElement element in recipeElements) {
            switch (element.Source) {
                case VendingRecipeElement.ItemSource.Fixed:
                    for (int i = 0; i < element.Count; i++) {
                        GameItem removedItem = interactorInventory.RemoveGameItem(element.Item);
                        if (removedItem == null) {
                            removedItem = vendorInventory.RemoveGameItem(element.Item);
                            if (removedItem == null) {
                                //Add inventory resetter to fix.
                                Debug.LogError($"Recipe removal failed: item missing from Interactor and Vendor");
                            } else {
                                vendorItems.Add(removedItem);
                            }
                        } else {
                            interactorItems.Add(removedItem);
                        }
                    }
                    break;
                case VendingRecipeElement.ItemSource.PortFirstItem:
                    for (int i = 0; i < element.Count; i++) {
                        GameItem removedItem = interactorInventory.RemoveGameItem();
                        if (removedItem == null) {
                            removedItem = vendorInventory.RemoveGameItem();
                            if (removedItem == null) {
                                //Add inventory resetter to fix.
                                Debug.LogError($"Recipe removal failed: item missing from Interactor and Vendor");
                            } else {
                                vendorItems.Add(removedItem);
                            }
                        } else {
                            interactorItems.Add(removedItem);
                        }
                    }
                    break;
            }
        }
    }
    // private static void RemoveItems(Inventory interactorInventory, Inventory vendorInventory, VendingRecipe recipe) {
    //     foreach (VendingRecipeElement element in recipe.Inputs) {
    //         switch (element.Source) {
    //             case VendingRecipeElement.ItemSource.Fixed: {
    //                     switch (element.Port) {
    //                         case VendingRecipeElement.ItemPort.Interactor:
    //                             interactorInventory.RemoveGameItem(element);
    //                             break;
    //                         case VendingRecipeElement.ItemPort.Vendor:
    //                             vendorInventory.RemoveGameItem(element);
    //                             break;
    //                         case VendingRecipeElement.ItemPort.Either:
    //                             for (int i = 0; i < element.Count; i++) {
    //                                 if (!interactorInventory.RemoveGameItem(element.Item)) {
    //                                     if (!vendorInventory.RemoveGameItem(element.Item)) {
    //                                         Debug.LogError($"Recipe removal failed: item missing from Interactor or Vendor");
    //                                     }
    //                                 }
    //                             }

    //                             break;
    //                     }
    //                     break;
    //                 }
    //             case VendingRecipeElement.ItemSource.PortFirstItem: {

    //                     switch (element.Port) {
    //                         case VendingRecipeElement.ItemPort.Interactor:
    //                             interactorInventory.RemoveGameItem();
    //                             break;
    //                         case VendingRecipeElement.ItemPort.Vendor:
    //                             vendorInventory.RemoveGameItem();
    //                             break;
    //                         case VendingRecipeElement.ItemPort.Either:
    //                             if (!interactorInventory.RemoveGameItem()) {
    //                                 if (!vendorInventory.RemoveGameItem()) {
    //                                     Debug.LogError($"Recipe removal failed: item missing from Interactor or Vendor");
    //                                 }
    //                             }

    //                             break;
    //                     }
    //                     break;
    //                 }
    //         }
    //     }
    // }
}
