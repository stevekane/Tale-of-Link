using System.Threading.Tasks;
using UnityEngine;

public class JumpPadAbility : ClassicAbility {
  public WorldSpaceController Controller;
  public float InstantLaunchDistance = .2f;
  public float TurnSpeed = 180f;

  JumpPad JumpPad;
  bool Jumped = false;

  void OnTriggerEnter(Collider c) {
    if (c.TryGetComponent(out JumpPad pad)) {
      pad.OnPopup += OnPopup;
    }
  }

  void OnTriggerStay(Collider c) {
    if (c.TryGetComponent(out JumpPad pad)) {
      if (!Jumped && AbilityManager.CanRun(Main) && (transform.position - pad.transform.position).sqrMagnitude < InstantLaunchDistance.Sqr()) {
        pad.Popup();
      }
    }
  }

  void OnTriggerExit(Collider c) {
    if (c.TryGetComponent(out JumpPad pad)) {
      pad.OnPopup -= OnPopup;
    }
  }

  void OnPopup(JumpPad jumpPad) {
    if (!Jumped && AbilityManager.CanRun(Main)) {
      Jumped = true;
      JumpPad = jumpPad;
      AbilityManager.Run(Main);
    }
  }

  public override async Task MainAction(TaskScope scope) {
    try {
      Controller.DesiredVelocity = Vector3.zero;
      Controller.PhysicsVelocity = Vector3.zero;
      var direction = Vector3.RotateTowards(JumpPad.transform.forward, Vector3.up, JumpPad.LaunchAngleDeg * Mathf.Deg2Rad, 0f);
      var velocity = JumpPad.LaunchSpeed * direction;
      var acceleration = velocity / LocalTime.FixedDeltaTime;
      Controller.Launch(acceleration);
      await scope.Until(() => !Controller.IsGrounded);
      while (!Controller.IsGrounded) {
        Controller.Rotation = Quaternion.RotateTowards(Controller.transform.rotation, JumpPad.transform.rotation, Time.fixedDeltaTime * TurnSpeed);
        await scope.Tick();
      }
    } finally {
      Controller.DirectMove = false;
      Jumped = false;
      JumpPad = null;
    }
  }
}
