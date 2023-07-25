using UnityEngine;

// TODO: Replace with SimpleAbility once it's imported.
public class TmpAbility : MonoBehaviour {
  public Player AbilityManager { get; internal set; }
  public virtual bool IsRunning { get; set; }
  public virtual void TryStart() { }
}