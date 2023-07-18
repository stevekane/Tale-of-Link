using UnityEngine;

public class PathMover : MonoBehaviour {
  [SerializeField] float RaiseHeight = 2;

  Vector3 p0;
  Vector3 p1;

  void Start() {
    p0 = transform.position;
    p1 = p0 + RaiseHeight * Vector3.up;
  }

  void Update() {
    transform.position = Vector3.Lerp(p0, p1, Mathf.Abs(Mathf.Sin(Time.time)));
  }
}