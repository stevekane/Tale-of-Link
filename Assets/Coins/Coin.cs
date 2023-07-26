using UnityEngine;

public class Coin : MonoBehaviour {
  public int Value;

  void OnTriggerEnter(Collider other) {
    if (other.TryGetComponent(out Collectbox collector)) {
      var coins = collector.Inventory.GetComponent<Coins>();
      if (!coins.IsFull) {
        coins.ChangeCurrent(Value);
        Destroy(gameObject);
      }
    }
  }
}