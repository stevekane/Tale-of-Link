using UnityEngine;

public class PathController : MonoBehaviour {
  public PathWanderer PathWanderer;
  public float MoveSpeed = 10f;

  void FixedUpdate() {
    var pos = transform.position;
    var rotation = transform.rotation;
    PathWanderer.Advance(ref pos, ref rotation, MoveSpeed);
    transform.position = pos;
    transform.rotation = rotation;
  }
}
