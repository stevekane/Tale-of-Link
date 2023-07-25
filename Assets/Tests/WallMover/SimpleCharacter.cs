using UnityEngine;

[DefaultExecutionOrder(-700)]
public class SimpleCharacter : MonoBehaviour {
  [SerializeField] float GroundSpeed = 5;
  [SerializeField] float WallSpeed = 3;
  [SerializeField] CapsuleCollider CapsuleCollider;
  [SerializeField] Mesh CapsuleMesh;
  [SerializeField] float EnterDistance = 1;
  [SerializeField] float ExitDistance = 1;
  [SerializeField] LayerMask LayerMask;

  [SerializeField] Controller Controller;
  [SerializeField] SimpleAbilityManager AbilityManager;
  [SerializeField] EnterWallSpace EnterWallSpace;
  [SerializeField] ExitWallSpace ExitWallSpace;
  [SerializeField] WallMover WallMover;

  Inputs Inputs;

  bool InTransition => AbilityManager.HasTag(AbilityTag.InSpaceTransition);
  bool InWall => AbilityManager.HasTag(AbilityTag.WallSpace);

  void Awake() {
    Inputs = new();
    Inputs.Enable();
  }

  void OnDestroy() {
    Inputs.Disable();
    Inputs.Dispose();
  }

  void FixedUpdate() {
    var wallMerge = Inputs.Player.North.WasPerformedThisFrame();
    if (wallMerge) {
      if (AbilityManager.CanRun(EnterWallSpace.Main)) {
        AbilityManager.Run(EnterWallSpace.Main);
      } else if (AbilityManager.CanRun(ExitWallSpace.Main)) {
        AbilityManager.Run(ExitWallSpace.Main);
      }
    }
    var move = Inputs.Player.Move.ReadValue<Vector2>();
    if (InWall) {
      WallMover.Move(WallSpeed * move.x);
    } else {
      Controller.Velocity = GroundSpeed * move.XZ();
      if (move.sqrMagnitude > 0)
        Controller.Forward = move.XZ();
    }
    if (Inputs.Player.L1.WasPerformedThisFrame()) {
      GetComponent<Magic>().Consume(25);
    }
    if (Inputs.Player.L2.WasPerformedThisFrame()) {
      GetComponent<Magic>().Restore(101);
    }
    if (Inputs.Player.R2.WasPerformedThisFrame()) {
    }
  }
}