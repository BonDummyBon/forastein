using System.Collections.Generic;
using UnityEngine;

public class TriggerSenser : MonoBehaviour {
    #region Fields

    private readonly HashSet<GameObject> gameObjectSet = new();

    #endregion

    #region Properties

    public IReadOnlyCollection<GameObject> Current => gameObjectSet;

    #endregion

    #region Unity Methods

    private void OnTriggerEnter2D(Collider2D collision) {
        gameObjectSet.Add(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision) {
        gameObjectSet.Remove(collision.gameObject);
    }

    #endregion
}
