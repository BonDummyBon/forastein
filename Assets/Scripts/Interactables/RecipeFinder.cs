using System.Collections.Generic;
using UnityEngine;

public class RecipeFinder {

    private const string LogPrefix = "RECIPE FINDER:";

    public static VendingRecipe Find(GameObject interactor, GameObject vendor, VendingRecipe[] recipeList) {
        Debug.Log($"{LogPrefix} Find called. interactor={DescribeObject(interactor)}, vendor={DescribeObject(vendor)}, recipeCount={(recipeList == null ? 0 : recipeList.Length)}");

        if (recipeList == null || recipeList.Length == 0) {
            Debug.LogWarning($"{LogPrefix} No recipes supplied. Returning null.");
            return null;
        }

        bool hasInteractorInventory = TryGetInventory(interactor, out Inventory interactorInventory);
        bool hasVendorInventory = TryGetInventory(vendor, out Inventory vendorInventory);
        if (!hasInteractorInventory || !hasVendorInventory) {
            Debug.LogWarning($"{LogPrefix} Could not resolve inventories. interactorInventoryFound={hasInteractorInventory}, vendorInventoryFound={hasVendorInventory}. Returning null.");
            return null;
        }

        Debug.Log($"{LogPrefix} Both inventories found. Finding compatible recipe.");
        VendingRecipe recipe = GetRecipe(interactorInventory, vendorInventory, recipeList);
        if (recipe == null) {
            Debug.Log($"{LogPrefix} No compatible recipe found. Returning null.");
            return null;
        }

        Debug.Log($"{LogPrefix} Found recipe: {recipe.name}. Returning recipe.");
        return recipe;
    }

    private static bool TryGetInventory(GameObject source, out Inventory inventory) {
        inventory = null;
        if (source == null) {
            Debug.LogWarning($"{LogPrefix} TryGetInventory failed: source GameObject is null.");
            return false;
        }

        bool found = source.TryGetComponent(out inventory);
        Debug.Log($"{LogPrefix} TryGetInventory source={DescribeObject(source)} found={found}");
        return found;
    }

    private static VendingRecipe GetRecipe(Inventory interactorInventory, Inventory vendorInventory, VendingRecipe[] recipeList) {
        Debug.Log($"{LogPrefix} GetRecipe start. interactorItems={interactorInventory.GetItemCount()}/{interactorInventory.Size}, vendorItems={vendorInventory.GetItemCount()}/{vendorInventory.Size}, vendorRecipes={recipeList.Length}");

        VendingRecipe bestRecipe = null;
        int precedence = -1;
        List<GameItem> interactorGameItems = new(interactorInventory.GetAllGameItems());
        List<GameItem> vendorGameItems = new(vendorInventory.GetAllGameItems());

        Debug.Log($"{LogPrefix} Snapshot item lists. interactorGameItems={interactorGameItems.Count}, vendorGameItems={vendorGameItems.Count}");

        foreach (VendingRecipe recipe in recipeList) {
            if (recipe == null) {
                Debug.LogWarning($"{LogPrefix} Encountered null recipe in list. Skipping.");
                continue;
            }

            Debug.Log($"{LogPrefix} Evaluating recipe {recipe.name} (precedence={recipe.Precedence}).");
            Dictionary<GameItem, int> interactorMemo = GetGameItemCounts(interactorGameItems);
            Dictionary<GameItem, int> vendorMemo = GetGameItemCounts(vendorGameItems);
            Debug.Log($"{LogPrefix} Built item count maps for recipe {recipe.name}. interactorUnique={interactorMemo.Count}, vendorUnique={vendorMemo.Count}");

            if (!inputCheck(recipe, interactorInventory, vendorInventory)) {
                Debug.Log($"{LogPrefix} Recipe {recipe.name} failed inputCheck.");
                continue;
            }
            Debug.Log($"{LogPrefix} Recipe {recipe.name} passed inputCheck.");

            if (!outputCheck(recipe, interactorInventory, vendorInventory)) {
                Debug.Log($"{LogPrefix} Recipe {recipe.name} failed outputCheck.");
                continue;
            }
            Debug.Log($"{LogPrefix} Recipe {recipe.name} passed outputCheck.");

            if (!itemCheck(recipe.InteractorInputs, interactorMemo)) {
                Debug.Log($"{LogPrefix} Recipe {recipe.name} failed interactor itemCheck.");
                continue;
            }
            Debug.Log($"{LogPrefix} Recipe {recipe.name} passed interactor itemCheck.");

            if (!itemCheck(recipe.VendorInputs, vendorMemo)) {
                Debug.Log($"{LogPrefix} Recipe {recipe.name} failed vendor itemCheck.");
                continue;
            }
            Debug.Log($"{LogPrefix} Recipe {recipe.name} passed vendor itemCheck.");

            Dictionary<GameItem, int> combinedMemo = new(interactorMemo);
            foreach (KeyValuePair<GameItem, int> kvp in vendorMemo) {
                combinedMemo[kvp.Key] = combinedMemo.GetValueOrDefault(kvp.Key, 0) + kvp.Value;
            }

            if (!itemCheck(recipe.EitherInputs, combinedMemo)) {
                Debug.Log($"{LogPrefix} Recipe {recipe.name} failed either itemCheck.");
                continue;
            }
            Debug.Log($"{LogPrefix} Recipe {recipe.name} passed either itemCheck.");

            int newPrecedence = recipe.Precedence;
            if (newPrecedence > precedence) {
                Debug.Log($"{LogPrefix} Recipe {recipe.name} becomes new best candidate. oldPrecedence={precedence}, newPrecedence={newPrecedence}");
                bestRecipe = recipe;
                precedence = newPrecedence;
            } else {
                Debug.Log($"{LogPrefix} Recipe {recipe.name} is valid but not selected. currentBestPrecedence={precedence}, candidatePrecedence={newPrecedence}");
            }
        }

        Debug.Log($"{LogPrefix} GetRecipe complete. bestRecipe={(bestRecipe == null ? "null" : bestRecipe.name)}, precedence={precedence}");
        return bestRecipe;
    }

    private static Dictionary<GameItem, int> GetGameItemCounts(List<GameItem> gameItems) {
        Debug.Log($"{LogPrefix} GetGameItemCounts start. sourceCount={(gameItems == null ? 0 : gameItems.Count)}");
        Dictionary<GameItem, int> counts = new(gameItems.Count);
        foreach (GameItem item in gameItems) {
            if (item == null) {
                Debug.LogWarning($"{LogPrefix} GetGameItemCounts encountered null item. Skipping.");
                continue;
            }

            counts.TryGetValue(item, out int count);
            counts[item] = count + 1;
        }

        Debug.Log($"{LogPrefix} GetGameItemCounts complete. uniqueItems={counts.Count}");
        return counts;
    }

    private static bool itemCheck(List<VendingRecipeElement> elements, Dictionary<GameItem, int> memo) {
        Debug.Log($"{LogPrefix} itemCheck start. elements={(elements == null ? 0 : elements.Count)}, memoUnique={(memo == null ? 0 : memo.Count)}");

        if (elements == null) {
            Debug.LogWarning($"{LogPrefix} itemCheck received null elements list. Treating as pass.");
            return true;
        }

        if (memo == null) {
            Debug.LogWarning($"{LogPrefix} itemCheck received null memo dictionary. Failing check.");
            return false;
        }

        foreach (VendingRecipeElement element in elements) {
            if (element.Source == VendingRecipeElement.ItemSource.PortFirstItem) {
                Debug.Log($"{LogPrefix} itemCheck skipping PortFirstItem element. element={element}");
                continue;
            }

            if (element == null || element.Item == null || element.Count <= 0) {
                Debug.LogWarning($"{LogPrefix} VendingRecipe has an element with a null item or non-positive count. This may cause issues at runtime.");
                continue;
            }

            memo.TryGetValue(element.Item, out int count);
            if (count < element.Count) {
                Debug.Log($"{LogPrefix} itemCheck failed for item={element.Item.name}. required={element.Count}, available={count}");
                return false;
            }

            Debug.Log($"{LogPrefix} itemCheck consumed item={element.Item.name}. required={element.Count}, availableBefore={count}, availableAfter={count - element.Count}");
            memo[element.Item] = count - element.Count;
        }

        Debug.Log($"{LogPrefix} itemCheck passed.");
        return true;
    }

    private static bool inputCheck(VendingRecipe recipe, Inventory interactorInventory, Inventory vendorInventory) {
        int interactorDif = interactorInventory.GetItemCount() - recipe.InteractorInputCount;
        int vendorDif = vendorInventory.GetItemCount() - recipe.VendorInputCount;
        Debug.Log($"{LogPrefix} inputCheck recipe={recipe.name}. interactorDif={interactorDif}, vendorDif={vendorDif}");

        if (interactorDif < 0 || vendorDif < 0) {
            Debug.Log($"{LogPrefix} inputCheck failed for recipe={recipe.name}. Not enough direct inputs.");
            return false;
        }

        int eitherDif = interactorDif + vendorDif - recipe.EitherInputCount;
        Debug.Log($"{LogPrefix} inputCheck recipe={recipe.name}. eitherDif={eitherDif}");
        if (eitherDif < 0) {
            Debug.Log($"{LogPrefix} inputCheck failed for recipe={recipe.name}. Not enough either inputs.");
            return false;
        }

        Debug.Log($"{LogPrefix} inputCheck passed for recipe={recipe.name}.");
        return true;
    }

    private static bool outputCheck(VendingRecipe recipe, Inventory interactorInventory, Inventory vendorInventory) {
        int interactorResultCount = interactorInventory.GetItemCount() + recipe.InteractorGain;
        int vendorResultCount = vendorInventory.GetItemCount() + recipe.VendorGain;

        Debug.Log($"{LogPrefix} outputCheck recipe={recipe.name}. interactorResult={interactorResultCount}/{interactorInventory.Size}, vendorResult={vendorResultCount}/{vendorInventory.Size}");

        if (interactorResultCount > interactorInventory.Size) {
            Debug.Log($"{LogPrefix} outputCheck failed for recipe={recipe.name}. Interactor inventory would overflow.\ninteractorResultCount={interactorResultCount}, interactorInventorySize={interactorInventory.Size}");
            return false;
        }

        if (vendorResultCount > vendorInventory.Size) {
            Debug.Log($"{LogPrefix} outputCheck failed for recipe={recipe.name}. Vendor inventory would overflow.\nvendorResultCount={vendorResultCount}, vendorInventorySize={vendorInventory.Size}");
            return false;
        }

        Debug.Log($"{LogPrefix} outputCheck passed for recipe={recipe.name}.");
        return true;
    }

    private static string DescribeObject(GameObject obj) {
        return obj == null ? "null" : $"{obj.name} ({obj.GetInstanceID()})";
    }
}
