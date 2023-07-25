using UnityEngine;

// Handles being able to pick up items on the ground.
public class ItemCollectable : MonoBehaviour {
  public GameObject Root;
  public ItemProto ItemProto;

  void OnTriggerEnter(Collider other) {
    if (other.TryGetComponent(out Collectbox collector)) {
      Debug.Log($"Deeeee doot-deet doodliDEE! You've picked up {ItemProto.name}");
      collector.Inventory.Add(ItemProto, 1);
      Root.Destroy();
    }
  }
}