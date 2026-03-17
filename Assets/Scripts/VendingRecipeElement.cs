using System;

[Serializable]
public class VendingRecipeElement {
    public GameItem Item;
    public int Count;

    public enum ItemPort {
        Interactor,
        Vendor,
        Either
    }

    public ItemPort Port;

    public enum ItemSource {
        Fixed,
        PortFirstItem
    }

    public ItemSource Source;
}
