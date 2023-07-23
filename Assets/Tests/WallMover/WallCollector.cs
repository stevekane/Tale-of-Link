using UnityEngine;

public class WallCollector : MonoBehaviour {
  void OnTriggerEnter(Collider c) {
    if (c.TryGetComponent(out WallCollectable collectable)) {
      gameObject.GetComponentInParent<Hearts>().Change(4);
      Destroy(c.gameObject);
      Debug.LogWarning("WallCollector assumes all collectables are hearts");
      // TODO: Just assume it's a heart for testing
      Debug.Log($"Found {collectable.name}");
    }
  }
}