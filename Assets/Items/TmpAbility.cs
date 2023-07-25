using UnityEngine;

// TODO: Replace with SimpleAbility once it's imported.
public class TmpAbility : MonoBehaviour {
  public Player AbilityManager { get; internal set; }
  public Player Player => AbilityManager;  // TODO
  public virtual bool IsRunning { get; set; }
  public virtual void TryStart() { }

  public enum Buttons { West, North, South };
  public Buttons DefaultButtonAssignment;
}