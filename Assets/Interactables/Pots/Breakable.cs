using System.Collections;
using UnityEngine;

public class Breakable : MonoBehaviour {
  [SerializeField] Collider UnbrokenCollider;
  [SerializeField] Rigidbody[] Pieces;
  [SerializeField] Timeval PersistenceDuration = Timeval.FromSeconds(1);
  [SerializeField] float ExplosionForce = 10;
  [SerializeField] DropTable DropTable;
  [SerializeField] Collider HurtBox;

  void Start() {
    GetComponent<Combatant>().OnHurt += Shatter;
  }

  void OnDestroy() {
    GetComponent<Combatant>().OnHurt -= Shatter;
  }

  void Shatter(HitEvent hitEvent) {
    var delta = transform.position - hitEvent.Attacker.transform.position;
    var direction = delta.normalized;
    HurtBox.enabled = false;
    StartCoroutine(ShatterRoutine(direction));
  }

  IEnumerator ShatterRoutine(Vector3 direction) {
    UnbrokenCollider.enabled = false;
    foreach (var piece in Pieces) {
      piece.GetComponent<Collider>().enabled = true;
      piece.useGravity = true;
      piece.isKinematic = false;
      piece.AddForce((direction + .25f * Random.onUnitSphere) * ExplosionForce, ForceMode.Impulse);
    }
    if (DropTable.TryGet(out var drop)) {
      Instantiate(drop, transform.position, Quaternion.LookRotation(Vector3.forward, Vector3.up));
    }
    yield return new WaitForSeconds(PersistenceDuration.Seconds);
    Destroy(gameObject);
  }
}