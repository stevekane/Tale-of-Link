using System;
using UnityEngine;

public class CameraFocusTarget : MonoBehaviour {
  public Bounds InnerBounds;
  public Bounds OuterBounds;
  public bool IsOutside = false;
  public float FollowFactor = 5f;

  Transform Follow;

  public void SetIsOutside(bool v) { IsOutside = v; }

  void Start() {
    Follow = transform.parent;
    transform.SetParent(null);
  }

  void FixedUpdate() {
      var pos = IsOutside ?
      ClosestOuterPoint(OuterBounds, Follow.position) :
      InnerBounds.ClosestPoint(Follow.position);
    transform.position = Vector3.Lerp(transform.position, pos, FollowFactor * Time.fixedDeltaTime);
  }

  Vector3 ClosestOuterPoint(Bounds b, Vector3 p) {
    if (b.Contains(p)) {
      // Push p out to the nearest vertical edge.
      var delta = p - b.center;
      var dir = delta.XZ().normalized;
      if (!b.IntersectRay(new Ray(b.center, dir), out var dist))
        Debug.LogError("Oops, no intersection");
      var pxz = b.center - dist*dir;
      p = new Vector3(pxz.x, p.y, pxz.z);
    }
    return p;
  }
}