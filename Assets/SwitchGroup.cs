using UnityEngine;

public class SwitchGroup : MonoBehaviour {
  public int State = 0;
  public Switch[] Switches;
  public SwitchBlock[] SwitchBlocks;
  public bool FindAmongChildren;

  void Awake() {
    if (FindAmongChildren) {
      Switches = GetComponentsInChildren<Switch>();
      SwitchBlocks = GetComponentsInChildren<SwitchBlock>();
    }
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
