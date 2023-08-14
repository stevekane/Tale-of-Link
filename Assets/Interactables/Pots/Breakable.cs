using System.Collections;
using UnityEngine;

public enum ShatterDirection {
  FromAttacker,
  Up
}

[RequireComponent(typeof(Combatant))]
public class Breakable : MonoBehaviour {
  [SerializeField] Collider UnbrokenCollider;
  [SerializeField] Rigidbody[] Pieces;
  [SerializeField] Timeval PersistenceDuration = Timeval.FromSeconds(1);
  [SerializeField] float ExplosionForce = 10;
  [SerializeField] DropTable DropTable;
  [SerializeField] Collider HurtBox;
  [SerializeField] ShatterDirection ShatterDirection;

  void Start() {
    GetComponent<Combatant>().OnHurt += Shatter;
  }

  void OnDestroy() {
    GetComponent<Combatant>().OnHurt -= Shatter;
  }

  void Shatter(HitEvent hitEvent) {
    var direction = ShatterDirection switch {
      ShatterDirection.FromAttacker => (transform.position-hitEvent.Attacker.transform.position).normalized,
      ShatterDirection.Up => Vector3.up
    };
    StartCoroutine(ShatterRoutine(direction));
  }

  IEnumerator ShatterRoutine(Vector3 direction) {
    HurtBox.enabled = false;
    UnbrokenCollider.enabled = false;
    foreach (var piece in Pieces) {
      piece.GetComponent<Collider>().enabled = true;
      piece.useGravity = true;
      piece.isKinematic = false;
      piece.AddForce((direction + .25f * Random.onUnitSphere) * ExplosionForce, ForceMode.Impulse);
    }
    if (DropTable && DropTable.TryGet(out var drop)) {
      Instantiate(drop, transform.position, Quaternion.LookRotation(Vector3.forward, Vector3.up));
    }
    yield return new WaitForSeconds(PersistenceDuration.Seconds);
    Destroy(gameObject);
  }
}