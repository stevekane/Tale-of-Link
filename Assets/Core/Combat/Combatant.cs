using System;
using UnityEngine;

public class Combatant : MonoBehaviour {
  public Action<HitEvent> OnHit;
  public Action<HitEvent> OnHurt;

  public void HandleHit(HitEvent hit) {
    OnHit?.Invoke(hit);
  }
  public void HandleHurt(HitEvent hit) {
    Debug.Log($"ouch {this} hit by {hit.Attacker}");
    OnHurt?.Invoke(hit);
  }
}