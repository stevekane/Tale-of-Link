using UnityEngine;

public class PathController : MonoBehaviour {
  public Waypoints Waypoints;
  public float MoveSpeed = 10f;
  public PathTraversal.Modes Mode;

  PathTraversal PathTraversal;

  void Awake() {
    PathTraversal = Waypoints.CreatePathTraversal(Mode);
    transform.position = Waypoints.Nodes[0].transform.position;
    transform.rotation = Waypoints.Nodes[0].transform.rotation;
  }

  void FixedUpdate() {
    var pos = transform.position;
    var rotation = transform.rotation;
    PathTraversal.Advance(ref pos, ref rotation, MoveSpeed);
    transform.position = pos;
    transform.rotation = rotation;
  }

  void OnDrawGizmosSelected() {
    var path = Waypoints.CreatePathTraversal(Mode);
    path.DrawGizmos();
  }
}
