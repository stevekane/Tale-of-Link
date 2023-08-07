using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SwitchTrigger : MonoBehaviour {
  public Switch2[] Switches;
  public UnityEvent AllSwitchesOn;

  void OnValidate() {
    Switches = GetComponentsInChildren<Switch2>();
  }
  void Start() {
    Switches.ForEach(s => s.GetComponent<Combatant>().OnHurt += (hit) => OnSwitchHurt(s));
  }

  void OnSwitchHurt(Switch2 s) {
    if (s.State == 1) return;
    s.SetSwitchState(1, true);
    var numOn = Switches.Count(s => s.State == 1);
    if (numOn == Switches.Length) {
      AllSwitchesOn?.Invoke();
    }
  }
}
