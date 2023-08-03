using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(GridLayoutGroup))]
public class KeyRing : MonoBehaviour {
  [SerializeField] Inventory Inventory;

  RectTransform RectTransform;
  GridLayoutGroup LayoutGroup;
  Dictionary<ItemProto, List<int>> ItemIndices = new();

  void Awake() {
    RectTransform = GetComponent<RectTransform>();
    LayoutGroup = GetComponent<GridLayoutGroup>();
    for (var i = 0; i < LayoutGroup.transform.childCount; i++)
      Destroy(LayoutGroup.transform.GetChild(i).gameObject);
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
    if (!itemProto.HUDGameObject)
      return;
    if (ItemIndices.TryGetValue(itemProto, out var indices)) {
      indices.Add(LayoutGroup.transform.childCount);
    } else {
      ItemIndices.Add(itemProto, new() { LayoutGroup.transform.childCount });
    }
    Instantiate(itemProto.HUDGameObject, LayoutGroup.transform);
  }

  void RemoveItem(ItemProto itemProto) {
    if (!itemProto.HUDGameObject)
      return;
    if (ItemIndices.TryGetValue(itemProto, out var indices)) {
      var lastIndex = indices[^1];
      indices.RemoveAt(indices.Count-1);
      Destroy(LayoutGroup.transform.GetChild(lastIndex).gameObject);
    }
  }
}