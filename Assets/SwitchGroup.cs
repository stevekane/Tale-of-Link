using UnityEngine;

public class SwitchGroup : MonoBehaviour {
  [Header("Initial State")]
  public bool RedRaised;
  [Header("Parts")]
  public Switch[] Switches;
  public SwitchBlock[] RedSwitchBlocks;
  public SwitchBlock[] BlueSwitchBlocks;
  [Header("Optional")]
  public bool FindAmongChildren;
  public GameObject RedSwitchesParent;
  public GameObject BlueSwitchesParent;

  void Awake() {
    if (FindAmongChildren) {
      Switches = GetComponentsInChildren<Switch>();
      RedSwitchBlocks = RedSwitchesParent.GetComponentsInChildren<SwitchBlock>();
      BlueSwitchBlocks = BlueSwitchesParent.GetComponentsInChildren<SwitchBlock>();
    }
    Switches.ForEach(s => s.GetComponent<Combatant>().OnHurt += OnSwitchHurt);
    Switches.ForEach(s => s.SetSwitchState(RedRaised, false));
    RedSwitchBlocks.ForEach(s => s.SetSwitchState(RedRaised, false));
    BlueSwitchBlocks.ForEach(b => b.SetSwitchState(!RedRaised, false));
  }

  void OnDestroy() {
    Switches.ForEach(s => s.GetComponent<Combatant>().OnHurt -= OnSwitchHurt);
  }

  void OnSwitchHurt(HitEvent _) {
    RedRaised = !RedRaised;
    Switches.ForEach(s => s.SetSwitchState(RedRaised, true));
    RedSwitchBlocks.ForEach(b => b.SetSwitchState(RedRaised, true));
    BlueSwitchBlocks.ForEach(b => b.SetSwitchState(!RedRaised, true));
  }
}
