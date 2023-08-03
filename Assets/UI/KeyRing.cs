using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(GridLayoutGroup))]
public class KeyRing : MonoBehaviour {
  [SerializeField] Inventory Inventory;

  RectTransform RectTransform;
  GridLayoutGroup LayoutGroup;

  void Awake() {
    RectTransform = GetComponent<RectTransform>();
    LayoutGroup = GetComponent<GridLayoutGroup>();
    Inventory.OnAddItem += AddItem;
    Inventory.OnRemoveItem += RemoveItem;
  }

  void OnDestroy() {
    Inventory.OnAddItem -= AddItem;
    Inventory.OnRemoveItem -= RemoveItem;
  }

  void LateUpdate() {
    var height = RectTransform.rect.height;
    var dimension = Mathf.Min(height, height);
    LayoutGroup.cellSize = new(dimension, dimension);
  }

  void AddItem(ItemProto itemProto) {
    Repopulate();
  }

  void RemoveItem(ItemProto itemProto) {
    Repopulate();
  }

  void Repopulate() {
    var childCount = LayoutGroup.transform.childCount;
    for (var i = 0; i < childCount; i++)
      Destroy(LayoutGroup.transform.GetChild(i).gameObject);
    foreach (var (itemProto, count) in Inventory.Items) {
      if (!itemProto.HUDGameObject)
        continue;
      for (var j = 0; j < count; j++) {
        Instantiate(itemProto.HUDGameObject, LayoutGroup.transform);
      }
    }
  }
}