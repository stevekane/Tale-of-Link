using UnityEngine;

public class Coin : MonoBehaviour {
  public bool PlayAnimation;
  public int Value;

  [SerializeField] GameObject Model;
  [SerializeField] string CollectionText;

  void OnTriggerEnter(Collider other) {
    if (other.TryGetComponent(out Collectbox collectBox)) {
      void Collect() {
        collectBox.Owner.GetComponent<Coins>().ChangeCurrent(Value);
        Destroy(gameObject);
      }
      collectBox.Collect(Model, CollectionText, Collect, PlayAnimation);
    }
  }
}