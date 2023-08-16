using System;
using System.Threading.Tasks;
using UnityEngine;

public class FallToDeath : ClassicAbility {
  [SerializeField] GameObject Model;
  [SerializeField] LayerMask FloorMask;
  [SerializeField] float MinDistance = 10f;
  [SerializeField] Timeval Starttime = Timeval.FromSeconds(0f);
  [SerializeField] Timeval Hangtime;
  [SerializeField] Timeval Falltime;
  [SerializeField] Timeval ReviveDelay = Timeval.FromSeconds(1f);
  [SerializeField] float FallSpeed = 9f;
  [SerializeField] int FallDamage = 2;

  WorldSpaceController Controller;
  Transform LastGround;
  Vector3 LastGroundedLocalPos;
  float LastGroundedTime;

  void Start() {
    AbilityManager.InitComponent(out Controller);
  }

  public override async Task MainAction(TaskScope scope) {
    var killable = AbilityManager.GetComponent<Killable>();
    var hearts = AbilityManager.GetComponent<Hearts>();
    var isPlayer = AbilityManager.GetComponent<Player>() != null;
    try {
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
      Model.SetActive(false);
      // Revive player, kill mob.
      if (isPlayer) {
        await scope.Delay(ReviveDelay);
        Controller.Position = LastGround.position + LastGroundedLocalPos - Controller.LedgeDirection.XZ();
        Controller.PhysicsVelocity = Vector3.zero;
        Model.SetActive(true);
        Model.transform.localScale = scale;
        hearts.ChangeCurrent(-FallDamage);
        //await scope.Ticks(5);
        LastGroundedLocalPos = Controller.Position - LastGround.position;
        LastGroundedTime = Time.fixedTime;
      } else {
        killable.Dying = true;
        killable.Dead = true;
        Destroy(killable.gameObject);
      }
    } finally {
      Controller.DirectMove = false;
    }
  }

  bool IsOverVoid() {
    if (Controller.IsGrounded)
      return false;
    if (Physics.SphereCast(Controller.Position + Controller.Motor.Capsule.center, Controller.Motor.Capsule.radius, Vector3.down, out var hit, MinDistance, FloorMask)) {
      return false;
    }
    return true;
  }

  protected override void FixedUpdate() {
    base.FixedUpdate();
    if (Controller.IsStableOnGround && !Controller.IsOnLedge && Controller.GroundCollider) {
      LastGround = Controller.GroundCollider.transform;
      LastGroundedLocalPos = Controller.Position - LastGround.position;
      LastGroundedTime = Time.fixedTime;
    }
    if (!IsRunning && IsOverVoid() && (Time.fixedTime - LastGroundedTime) > Starttime.Seconds) {
      Debug.Log($"{AbilityManager} falling to death at {Time.fixedTime}");
      AbilityManager.Run(Main);
    }
  }

  private void OnGUI() {
    if (LastGround == null) return;
    var pos = LastGround.position + LastGroundedLocalPos;
    GUIExtensions.DrawLine(pos, pos + Vector3.up, 3);
  }
}