using UnityEngine;

public class Hurtbox : MonoBehaviour {
  public Combatant Owner;

  void Awake() {
    Owner = Owner ?? GetComponentInParent<Combatant>();
  }
}