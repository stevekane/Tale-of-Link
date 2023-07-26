using UnityEngine;

[DefaultExecutionOrder(-700)]
public class SimpleCharacter : MonoBehaviour {
  [SerializeField] SimpleAbilityManager AbilityManager;
  [SerializeField] EnterWallSpace EnterWallSpace;
  [SerializeField] ExitWallSpace ExitWallSpace;
  [SerializeField] WorldSpaceMove WorldSpaceMove;
  [SerializeField] WallSpaceMove WallSpaceMove;

  Inputs Inputs;

  void Awake() {
    Inputs = new();
    Inputs.Enable();
  }

  void OnDestroy() {
    Inputs.Disable();
    Inputs.Dispose();
  }

  void FixedUpdate() {
    // This is evaluation of poetential inputs

    // This is response to inputs
    var wallMerge = Inputs.Player.North.WasPerformedThisFrame();
    if (wallMerge) {
      if (AbilityManager.CanRun(EnterWallSpace.Main)) {
        AbilityManager.Run(EnterWallSpace.Main);
      } else if (AbilityManager.CanRun(ExitWallSpace.Main)) {
        AbilityManager.Run(ExitWallSpace.Main);
      }
    }
    var move = Inputs.Player.Move.ReadValue<Vector2>();
    if (AbilityManager.CanRun(WallSpaceMove.Move)) {
      AbilityManager.Run(WallSpaceMove.Move, move);
    } else if (AbilityManager.CanRun(WorldSpaceMove.Move)) {
      AbilityManager.Run(WorldSpaceMove.Move, new(move.x, 0, move.y));
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