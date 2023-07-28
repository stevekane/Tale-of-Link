using UnityEngine;

public class Heart : MonoBehaviour {
  void OnTriggerEnter(Collider other) {
    if (other.TryGetComponent(out Collectbox collector)) {
      var hearts = collector.Inventory.GetComponent<Hearts>();
      if (!hearts.IsFull) {
        hearts.ChangeCurrent(4);
        Destroy(gameObject);
      }
    }
  }
}