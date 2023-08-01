using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour {
  public Hurtbox Hurtbox;
  public Renderer Renderer;
  public Material MaterialZero;
  public Material MaterialOne;
  public int State = 0;
  public List<SwitchBlock> AttachedBlocks;

  private void Awake() {
    GetComponent<Combatant>().OnHurt += OnHurt;
    Renderer.material = State == 1 ? MaterialOne : MaterialZero;
    AttachedBlocks.ForEach(b => b.SetSwitchState(State, false));
  }

  void OnHurt(HitEvent _) {
    State = (State+1)%2;
    Renderer.material = State == 1 ? MaterialOne : MaterialZero;
    AttachedBlocks.ForEach(b => b.SetSwitchState(State, true));
  }
}
