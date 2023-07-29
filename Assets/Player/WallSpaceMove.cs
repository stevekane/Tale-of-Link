using UnityEngine;

public class WallSpaceMove : Ability {
  [SerializeField] float Speed = 3;
  [SerializeField] WallSpaceController WallSpaceController;

  public AbilityAction<Vector3> Move;

  void Awake() {
    Move.Ability = this;
    Move.Listen(OnMove);
  }

  void OnMove(Vector3 v) {
    Debug.Log($"WallSpaceMove {Timeval.TickCount}");
    WallSpaceController.Move(Speed * v.x);
  }
}