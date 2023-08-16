using System;
using System.Threading.Tasks;
using UnityEngine;

// TODO: player version. maybe just a diff script
public class FallToDeath : ClassicAbility {
  [SerializeField] GameObject Model;
  [SerializeField] LayerMask FloorMask;
  [SerializeField] float MinDistance = 10f;
  [SerializeField] Timeval Hangtime;
  [SerializeField] Timeval Falltime;
  [SerializeField] float FallSpeed = 3f;

  WorldSpaceController Controller;
  void Start() {
    AbilityManager.InitComponent(out Controller);
  }

  public override async Task MainAction(TaskScope scope) {
    var killable = AbilityManager.GetComponent<Killable>();
    try {
      killable.Dying = true;
      // Hack to force DirectMove even when other abilities (Knockback) turn it off.
      scope.Start(Waiter.Repeat(() => Controller.DirectMove = true), TaskRunner);
      var scale = Model.transform.localScale;
      await scope.ForDuration(Hangtime, pct => {
        var s = .1f*Mathf.Sin(pct*50f);
        Model.transform.localScale = Vector3.Scale(scale, new Vector3(1+s, 1-s, 1+s));
      });
      await scope.ForDuration(Falltime, pct => {
        Controller.Position += Time.fixedDeltaTime * FallSpeed * Vector3.down;
        Model.transform.localScale = scale * (1f - pct);
      });
      killable.Dead = true;
      Destroy(killable.gameObject);
    } finally {
    }
  }

  bool IsOverVoid() {
    if (Controller.IsGrounded)
      return false;
    if (Physics.SphereCast(Controller.Position, Controller.Motor.Capsule.radius, Vector3.down, out var hit, MinDistance, FloorMask)) {
      return false;
    }
    return true;
  }

  protected override void FixedUpdate() {
    base.FixedUpdate();
    if (!IsRunning && IsOverVoid()) {
      AbilityManager.Run(Main);
    }
  }
}