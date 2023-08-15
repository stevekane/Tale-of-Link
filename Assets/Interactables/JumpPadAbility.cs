using System.Threading.Tasks;
using UnityEngine;

public class JumpPadAbility : ClassicAbility {
  public WorldSpaceController Controller;
  public float InstantLaunchDistance = .2f;
  public float TurnSpeed = 180f;

  JumpPad JumpPad;
  bool Jumped;

  void OnTriggerEnter(Collider c) {
    if (c.TryGetComponent(out JumpPad pad) && !JumpPad) {
      JumpPad = pad;
      JumpPad.OnPopup += OnPopup;
      Jumped = false;
    }
  }

  void OnTriggerExit(Collider c) {
    if (c.TryGetComponent(out JumpPad pad) && pad == JumpPad) {
      JumpPad.OnPopup -= OnPopup;
      JumpPad = null;
    }
  }

  void OnPopup() {
    if (JumpPad && !Jumped && AbilityManager.CanRun(Main)) {
      Jumped = true;
      AbilityManager.Run(Main);
    }
  }

  public override async Task MainAction(TaskScope scope) {
    try {
      var pad = JumpPad;  // cache it because it's cleared as soon as we're off it.
      //Controller.Position = JumpPad.transform.position;
      Controller.DesiredVelocity = Vector3.zero;
      Controller.PhysicsVelocity = Vector3.zero;
      var direction = Vector3.RotateTowards(pad.transform.forward, Vector3.up, pad.LaunchAngleDeg * Mathf.Deg2Rad, 0f);
      var velocity = pad.LaunchSpeed * direction;
      var acceleration = velocity / Time.fixedDeltaTime;
      Controller.Launch(acceleration);
      await scope.Until(() => !Controller.IsGrounded);
      while (!Controller.IsGrounded) {
        Controller.Rotation = Quaternion.RotateTowards(Controller.transform.rotation, pad.transform.rotation, Time.fixedDeltaTime * TurnSpeed);
        await scope.Tick();
      }
    } finally {
      Controller.DirectMove = false;
      Jumped = false;
    }
  }

  protected override void FixedUpdate() {
    base.FixedUpdate();
    if (JumpPad && !Jumped && AbilityManager.CanRun(Main) && (transform.position - JumpPad.transform.position).sqrMagnitude < InstantLaunchDistance.Sqr()) {
      //Jumped = true;
      JumpPad.Popup();
      //AbilityManager.Run(Main);
    }
  }
}
