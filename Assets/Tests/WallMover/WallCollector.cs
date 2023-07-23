using UnityEngine;

public class WallCollector : MonoBehaviour {
  void OnTriggerEnter(Collider c) {
    if (c.TryGetComponent(out Heart heart)) {
      var hearts = gameObject.GetComponentInParent<Hearts>();
      if (!hearts.IsFull) {
        hearts.ChangeCurrent(4);
        Destroy(c.gameObject);
      }
    } else if (c.TryGetComponent(out Coin coin)) {
      var wallet = gameObject.GetComponentInParent<Coins>();
      if (!wallet.IsFull) {
        wallet.Change(coin.Value);
        Destroy(c.gameObject);
      }
    } else {
      // TODO: Segment colliders often collide with themselves
      // Debug.LogError($"Unknown collider {c.name}");
    }
  }
}