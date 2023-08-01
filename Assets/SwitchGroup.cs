using System.Collections.Generic;
using UnityEngine;

public class SwitchGroup : MonoBehaviour {
  public int State = 0;
  public List<Switch> Switches;
  public List<SwitchBlock> SwitchBlocks;

  void Awake() {
    Switches.ForEach(s => s.GetComponent<Combatant>().OnHurt += OnSwitchHurt);
    Switches.ForEach(s => s.SetSwitchState(State, false));
    SwitchBlocks.ForEach(b => b.SetSwitchState(State, false));
  }

  void OnSwitchHurt(HitEvent _) {
    State = (State+1)%2;
    Switches.ForEach(s => s.SetSwitchState(State, true));
    SwitchBlocks.ForEach(b => b.SetSwitchState(State, true));
  }
}
