using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Item", menuName = "Game Item")]
public class GameItem : ScriptableObject {
    #region Fields

    public string itemName;
    public Sprite itemIcon;
    public List<string> itemTags;

    #endregion

    public override string ToString() {
        return itemName;
    }
}
