using UnityEngine;

public enum ShatterDirection {
  FromAttacker,
  Up,
  Forward,
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
  [SerializeField] bool ShatterOnHurt = true;
  [SerializeField] bool ShatterOnHit = false;

  void Start() {
    if (ShatterOnHurt)
      GetComponent<Combatant>().OnHurt += Shatter;
    if (ShatterOnHit)
      GetComponent<Combatant>().OnHit += Shatter;
  }

  void OnDestroy() {
    GetComponent<Combatant>().OnHurt -= Shatter;
    GetComponent<Combatant>().OnHit -= Shatter;
  }

  void Shatter(HitEvent hitEvent) {
    var direction = ShatterDirection switch {
      ShatterDirection.FromAttacker => (transform.position-hitEvent.Attacker.transform.position).normalized,
      ShatterDirection.Up => Vector3.up,
      ShatterDirection.Forward => transform.forward
    };
    HurtBox.enabled = false;
    UnbrokenCollider.enabled = false;
    foreach (var piece in Pieces) {
      piece.gameObject.SetActive(true);
      piece.transform.SetParent(null, worldPositionStays: true);
      piece.GetComponent<Collider>().enabled = true;
      piece.useGravity = true;
      piece.isKinematic = false;
      piece.AddForce((direction + .25f * Random.onUnitSphere) * ExplosionForce, ForceMode.Impulse);
    }
    if (DropTable && DropTable.TryGet(out var drop)) {
      Instantiate(drop, transform.position, Quaternion.LookRotation(Vector3.forward, Vector3.up));
    }
    Pieces.ForEach(piece => Destroy(piece.gameObject, PersistenceDuration.Seconds));
    Destroy(gameObject);
  }
}