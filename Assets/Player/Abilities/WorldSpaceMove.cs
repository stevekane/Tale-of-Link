using System;
using UnityEngine;

public class WorldSpaceMove : Ability {
  public float Speed = 5;
  public float LedgeStopDistance = 1f;

  public AbilityAction<Vector3> Move;
  public Action OnLedge;

  LocalTime LocalTime;
  WorldSpaceController Controller;

  void Start() {
    Move.Ability = this;
    Move.Listen(OnMove);
    AbilityManager.InitComponent(out LocalTime);
    AbilityManager.InitComponent(out Controller);
  }

  void OnMove(Vector3 stick) {
    var dir = stick.XZ();
    if (LedgeStopDistance > 0f) {
      var probePos = Controller.Position + LedgeStopDistance*dir.normalized;
      if (Controller.Motor.CharacterCollisionsRaycast(probePos + .5f*Vector3.up, Vector3.down, .6f, out var hit, null) == 0) {
        // Heading towards a ledge.
        OnLedge?.Invoke();
        var voidPos = probePos - .1f*Vector3.up;
        if (Controller.Motor.CharacterCollisionsRaycast(voidPos, -dir.normalized, LedgeStopDistance, out var ledgeHit, null) > 0) {
          var towardsLedge = Vector3.Project(dir, ledgeHit.normal.XZ());
          dir -= towardsLedge.XZ();
        } else {
          Debug.LogWarning($"No ledge detected between ground and void.");
          //dir = Vector3.zero;
        }
      } else if (Controller.Motor.CharacterCollisionsRaycast(Controller.Position + .1f*Vector3.up, dir.normalized, LedgeStopDistance, out var wallHit, null) > 0) {
        // Heading towards a wall.
        OnLedge?.Invoke();  // ledge edge, potato potato
        var towardsWall = Vector3.Project(dir, -wallHit.normal.XZ());
        dir -= towardsWall.XZ();
      }
    }
    Controller.MaxMoveSpeed = Speed;
    if (LocalTime.TimeScale > 0) {
      Controller.DesiredVelocity += LocalTime.TimeScale * Speed * dir;
      if (stick.sqrMagnitude > 0 && AbilityManager.HasTags(AbilityTag.CanRotate))
        Controller.Forward = stick;
    }
  }
}