using UnityEngine;

public class Heart : MonoBehaviour {
  [SerializeField] GameObject Model;
  [SerializeField] string CollectionText;

  void OnTriggerEnter(Collider other) {
    if (other.TryGetComponent(out Collectbox collectBox)) {
      void Collect() {
        collectBox.Owner.GetComponent<Hearts>().ChangeCurrent(4);
        Destroy(gameObject);
      }
      collectBox.Collect(Model, CollectionText, Collect);
    }
  }
}