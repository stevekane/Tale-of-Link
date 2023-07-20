using System;
using UnityEngine;

public class Combatant : MonoBehaviour {
  public Action<Combatant> OnHit;
  public Action<Combatant> OnHurt;

  public void HandleHit(Combatant victim) {
    OnHit?.Invoke(victim);
  }
  public void HandleHurt(Combatant attacker) {
    OnHurt?.Invoke(attacker);
  }
}