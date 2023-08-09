using UnityEngine;

// Handles being able to pick up items on the ground.
public class ItemCollectable : MonoBehaviour {
  [SerializeField] GameObject Model;
  [SerializeField] string CollectionText;

  public GameObject Root;
  public ItemProto ItemProto;
  public bool PlayAnimation;

  void OnTriggerEnter(Collider other) {
    if (other.TryGetComponent(out Collectbox collectBox)) {
      void Collect() {
        collectBox.Owner.GetComponent<Inventory>().Add(ItemProto);
        Root.Destroy();
      }
      collectBox.Collect(Model, CollectionText, Collect, PlayAnimation);
    }
  }
}