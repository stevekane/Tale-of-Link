using System.Threading.Tasks;
using UnityEngine;

// Fundamental ability concepts:
// IsRunning: abilities and systems want to know when they/other abilities are running
// CanRun: can this ability start given the state of the character/game?
// maybe CanBeCancelled: can this ability be cancelled by the character?
// maybe CanBeInterrupted: can this ability be interrupted by an event? alternative could just be each ability listens for the events that can interrupt it.
public class Sword : TmpAbility {
  public bool CanRun => !IsRunning;  // TODO: AbilityManager handles it
  //public bool CanRun => !IsRunning && !Player.Hammer.IsRunning;

  public Vector3 AttachOffsetTODO;
  public GameObject Model;
  public Hitbox Hitbox;
  TaskScope Scope = new();

  void Start() {
    if (Player) {
      // TODO: Attach to player's hand.
      transform.localPosition = AttachOffsetTODO;
    }
  }
  void OnDestroy() => Scope.Dispose();

  public override void TryStart() {
    TryStart(Run);
  }

  void TryStart(TaskFunc func) {
    if (CanRun)
      Scope.Start(func);
  }

  async Task Run(TaskScope scope) {
    try {
      IsRunning = true;
      Model.SetActive(true);
      Hitbox.EnableCollision = true;
      await scope.Seconds(.5f);
    } finally {
      IsRunning = false;
      Model.SetActive(false);
      Hitbox.EnableCollision = false;
    }
  }
}