using System.Threading.Tasks;
using UnityEngine;

public class Hammer : TmpAbility {
  public bool CanRun => !IsRunning; // TODO
  //public bool CanRun => !IsRunning && !Player.Sword.IsRunning;

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