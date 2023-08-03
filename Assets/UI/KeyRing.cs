using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(GridLayoutGroup))]
public class KeyRing : MonoBehaviour {
  [SerializeField] Inventory Inventory;

  GridLayoutGroup LayoutGroup;
  RectTransform RectTransform;

  void Awake() {
    LayoutGroup = GetComponent<GridLayoutGroup>();
    RectTransform = GetComponent<RectTransform>();
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
    Debug.Log($"Added {itemProto.name}");
  }

  void RemoveItem(ItemProto itemProto) {
    if (!itemProto.HUDGameObject)
      return;
    Debug.Log($"Removed {itemProto.name}");
  }
}