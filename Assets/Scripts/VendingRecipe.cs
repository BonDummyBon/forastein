using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New VendingRecipe", menuName = "Vending Recipe")]
public class VendingRecipe : ScriptableObject {
    #region Fields

    public string RecipeName;
    public float WaitTime;
    public WaitForSeconds CachedWaitTime;
    public List<VendingRecipeElement> Inputs;
    public List<VendingRecipeElement> Outputs;

    #endregion

    [NonSerialized] public int InteractorInputCount = 0;
    [NonSerialized] public int VendorInputCount = 0;
    [NonSerialized] public int EitherInputCount = 0;
    [NonSerialized] public int InteractorGain = 0;
    [NonSerialized] public int VendorGain = 0;

    [NonSerialized] public List<VendingRecipeElement> InteractorInputs;
    [NonSerialized] public List<VendingRecipeElement> VendorInputs;
    [NonSerialized] public List<VendingRecipeElement> EitherInputs;
    [NonSerialized] public List<VendingRecipeElement> InteractorOutputs;
    [NonSerialized] public List<VendingRecipeElement> VendorOutputs;

    private void OnValidate() {
        CachedWaitTime = new WaitForSeconds(WaitTime);
        Inputs ??= new List<VendingRecipeElement>();
        Outputs ??= new List<VendingRecipeElement>();
        FixPorts(Inputs);
        FixPorts(Outputs);
        UpdateInputCounts();
        UpdateGains();
    }

    public int Precedence => Inputs.Count;

    public override string ToString() {
        return RecipeName;
    }

    private void UpdateInputCounts() {
        InteractorInputCount = 0;
        VendorInputCount = 0;
        EitherInputCount = 0;
        InteractorInputs = new List<VendingRecipeElement>();
        VendorInputs = new List<VendingRecipeElement>();
        EitherInputs = new List<VendingRecipeElement>();


        foreach (VendingRecipeElement element in Inputs) {
            if (element.Port == VendingRecipeElement.ItemPort.Interactor) {
                InteractorInputCount += element.Count;
                InteractorInputs.Add(element);
            } else if (element.Port == VendingRecipeElement.ItemPort.Vendor) {
                VendorInputCount += element.Count;
                VendorInputs.Add(element);
            } else if (element.Port == VendingRecipeElement.ItemPort.Either) {
                EitherInputCount += element.Count;
                EitherInputs.Add(element);
            }
        }

        if (InteractorInputCount > 1) {
            Debug.LogWarning($"VendingRecipe {name} has more than 1 interactor input item. Player Inventory size is 1, so this may cause issues at runtime.");
        }
    }

    private void UpdateGains() {
        InteractorGain = 0;
        VendorGain = 0;
        InteractorOutputs = new List<VendingRecipeElement>();
        VendorOutputs = new List<VendingRecipeElement>();

        int interactorOutputCount = 0;
        int vendorOutputCount = 0;
        foreach (VendingRecipeElement element in Outputs) {
            if (element.Port == VendingRecipeElement.ItemPort.Interactor) {
                interactorOutputCount += element.Count;
                InteractorOutputs.Add(element);
            } else if (element.Port == VendingRecipeElement.ItemPort.Vendor) {
                vendorOutputCount += element.Count;
                VendorOutputs.Add(element);
            } else if (element.Port == VendingRecipeElement.ItemPort.Either) {
                Debug.LogWarning($"VendingRecipe {name} has an output element with port Either. This may cause issues at runtime.");
            }
        }

        InteractorGain = interactorOutputCount - InteractorInputCount;
        VendorGain = vendorOutputCount - VendorInputCount;

        if (InteractorGain < -1) {
            Debug.LogWarning($"VendingRecipe {name} loses 2+ items for the interaction. Player Inventory size is 1, so this may cause issues at runtime.");
        }
    }

    private void FixPorts(List<VendingRecipeElement> elements) {
        foreach (VendingRecipeElement element in elements) {
            switch (element.Source) {
                case VendingRecipeElement.ItemSource.Fixed:
                    if (element.Item == null) {
                        Debug.LogWarning($"VendingRecipe {name} has an element with a null item. This may cause issues at runtime.");
                    }
                    if (element.Count <= 0) {
                        Debug.LogWarning($"VendingRecipe {name} has an element with a non-positive count. This may cause issues at runtime.");
                    }
                    break;
                case VendingRecipeElement.ItemSource.PortFirstItem:
                    element.Count = 1;
                    element.Item = null;
                    break;
            }
        }
    }
}
