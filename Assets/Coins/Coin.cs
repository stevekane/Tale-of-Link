using UnityEngine;

public class Coin : MonoBehaviour {
  public bool PlayAnimation;
  public int Value;

  [SerializeField] GameObject Model;

  void OnTriggerEnter(Collider other) {
    if (other.TryGetComponent(out Collectbox collectBox)) {
      void Collect() {
        collectBox.Owner.GetComponent<Coins>().ChangeCurrent(Value);
        Destroy(gameObject);
      }
      collectBox.Collect(Model, Collect, PlayAnimation);
    }
  }
}