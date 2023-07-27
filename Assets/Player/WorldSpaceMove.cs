using UnityEngine;

public class WorldSpaceMove : Ability {
  [SerializeField] WorldSpaceController WorldSpaceController;
  [SerializeField] float Speed = 5;

  Vector3 Velocity;

  public AbilityAction<Vector3> Move;

  void Awake() {
    Move.Ability = this;
    Move.Listen(OnMove);
  }

  void OnMove(Vector3 stick) {
    var v0 = Velocity.XZ();
    var v1 = Speed * stick.XZ();
    var dvxz = v1 - v0;
    var magnitude = Mathf.Min(2 * Speed, dvxz.magnitude);
    var dv = magnitude * dvxz.normalized;
    Velocity = v1;
    WorldSpaceController.Velocity += dv;
    if (stick.sqrMagnitude > 0)
      WorldSpaceController.Forward = stick;
  }
}
