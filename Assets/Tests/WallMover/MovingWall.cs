using UnityEngine;

[DefaultExecutionOrder(2)]
public class MovingWall : MonoBehaviour {
  [SerializeField] Vector3 Offset;
  [SerializeField] float Speed = 1;

  Vector3 p0;
  Vector3 p1;

  // TODO: Handle RotationDelta as well?
  public Vector3 MotionDelta;
  public Vector3 PreviousMotionDelta;

  void Start() {
    p0 = transform.position;
    p1 = transform.TransformPoint(Offset);
  }

  void FixedUpdate() {
    var current = transform.position;
    var next = Vector3.Lerp(p0, p1, Mathf.Abs(Mathf.Sin(Time.time * Speed)));
    transform.position = next;
    PreviousMotionDelta = MotionDelta;
    MotionDelta = next-current;
  }

  void OnDrawGizmos() {
    Gizmos.DrawLine(transform.position, transform.TransformPoint(Offset));
  }
}