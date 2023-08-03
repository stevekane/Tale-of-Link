using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {
  public HitConfig HitConfig;
  public float InitialSpeed = 10;
  public Combatant Attacker;

  public static Projectile Fire(Projectile prefab, Combatant attacker, Vector3 pos, Quaternion rotation) {
    var p = Instantiate(prefab, pos, rotation);
    p.Attacker = attacker;
    return p;
  }

  void Start() {
    GetComponent<Rigidbody>().AddForce(InitialSpeed*transform.forward, ForceMode.VelocityChange);
  }

  void OnTriggerEnter(Collider other) { // MP: This seems to be called for child objects too?
    if (other.gameObject.TryGetComponent(out Hurtbox hb)) {
      hb.ProcessHit(Attacker, HitConfig);
      Destroy(gameObject);
    }
  }

  void OnCollisionEnter(Collision collision) {
    Destroy(gameObject);
  }
}