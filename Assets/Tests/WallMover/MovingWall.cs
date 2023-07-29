using UnityEngine;

[DefaultExecutionOrder(2)]
public class MovingWall : MonoBehaviour {
  [SerializeField] Vector3 Offset;
  [SerializeField] float Speed = 1;

  Vector3 p0;
  Vector3 p1;

  // TODO: Handle RotationDelta as well?
  public Vector3 MotionDelta;

  void Start() {
    p0 = transform.position;
    p1 = transform.TransformPoint(Offset);
  }

  void FixedUpdate() {
    Debug.Log($"MovingWall {Timeval.TickCount}");
    var current = transform.position;
    var next = Vector3.Lerp(p0, p1, Mathf.Abs(Mathf.Sin(Time.time * Speed)));
    transform.position = next;
    MotionDelta = next-current;
  }

  void OnDrawGizmos() {
    Gizmos.DrawLine(transform.position, transform.TransformPoint(Offset));
  }
}