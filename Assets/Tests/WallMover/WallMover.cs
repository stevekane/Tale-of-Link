using System;
using System.Collections.Generic;
using UnityEngine;

public class WallMover : MonoBehaviour {
  [Tooltip("Max number of checks")]
  public int MaxSearchCount = 10;
  [Tooltip("Distance along the wall to check for next contact")]
  public float SampleSpacing = .25f;
  [Tooltip("Distance from wall to cast rays from")]
  public float WallOffset = .05f;
  public float MaxDistance => SampleSpacing + WallOffset;
  public bool ShowHits;
  public Collider Collider;

  public static List<RaycastHit> Corners(List<RaycastHit> hits) {
    List<RaycastHit> corners = new List<RaycastHit>();
    for (int i = 0; i < hits.Count - 1; i++) {
      Vector3 currentPoint = hits[i].point;
      Vector3 nextPoint = hits[i + 1].point;
      Vector3 currentNormal = hits[i].normal;
      Vector3 nextNormal = hits[i + 1].normal;
      if (Vector3.Angle(currentNormal, nextNormal) > 5) {
        Vector3 corner = FindIntersection(hits[i], hits[i+1]);
        corners.Add(new RaycastHit() { point = corner, normal = currentNormal });
      }
    }
    return corners;
  }

  private static Vector3 FindIntersection(RaycastHit hit1, RaycastHit hit2) {
    // Calculate the slopes (m1 and m2)
    Vector3 tangent1 = Vector3.Cross(hit1.normal, Vector3.up);
    Vector3 tangent2 = Vector3.Cross(hit2.normal, Vector3.up);
    float m1 = tangent1.z / tangent1.x;
    float m2 = tangent2.z / tangent2.x;

    // Calculate the intercepts (b1 and b2)
    float b1 = hit1.point.z - m1 * hit1.point.x;
    float b2 = hit2.point.z - m2 * hit2.point.x;

    // Calculate the intersection point
    float x = (b2 - b1) / (m1 - m2);
    float z = m1 * x + b1;

    // Since all points are assumed to be in the same XZ plane (heights are the same along the Y axis),
    // we can take the y coordinate from any of the RaycastHit points.
    return new Vector3(x, hit1.point.y, z);
  }

  public static RaycastHit GetNewPositionAndNormal(List<RaycastHit> hits, float distance) {
    Vector3 lastPoint = hits[0].point;
    Vector3 normal = hits[0].normal;
    for (int i = 1; i < hits.Count; i++) {
      Vector3 currentPoint = hits[i].point;
      float segmentDistance = Vector3.Distance(lastPoint, currentPoint);
      if (segmentDistance >= distance) {
        Vector3 newPosition = Vector3.Lerp(lastPoint, currentPoint, distance / segmentDistance);
        return new RaycastHit() { point = newPosition, normal = normal };
      }
      distance -= segmentDistance;
      lastPoint = currentPoint;
      normal = hits[i].normal;
    }
    return new RaycastHit() { point = hits[hits.Count - 1].point, normal = hits[hits.Count - 1].normal };
  }

  List<RaycastHit> Hits = new();
  void FixedUpdate() {
    var position = transform.position;
    var normal = transform.forward;
    var tangent = Vector3.Cross(transform.forward, Vector3.up);
    var rayOrigin = position + WallOffset * normal;
    var rayDirection = -normal;
    var didHit = Physics.Raycast(rayOrigin, rayDirection, out var hit, MaxDistance);
    Debug.DrawRay(rayOrigin, MaxDistance * rayDirection, didHit ? Color.green : Color.black);
    Hits.Clear();
    if (didHit) {
      Collider = hit.collider;
      Hits.Add(hit);
      for (var i = 0; i < MaxSearchCount; i++) {
        var nextHit = FindNext(hit);
        if (nextHit.HasValue) {
          Hits.Add(nextHit.Value);
          hit = nextHit.Value;
        } else {
          break;
        }
      }
    }
    // IMPORTANT: We are assuming for the moment the path is always long enough
    // in reality, there are lots of reasons the path may be too short. In those cases,
    // restrict motion to the length of the path
    var speed = 1;
    var distance = speed * Time.fixedDeltaTime;
    var corners = Corners(Hits);
    Debug.Log(corners.Count);
    corners.Insert(0, Hits[0]);
    corners.Add(Hits[Hits.Count - 1]);
    foreach (var point in corners) {
      Debug.DrawRay(point.point, point.normal, Color.blue);
    }
    var newHit = GetNewPositionAndNormal(corners, distance);
    // transform.SetPositionAndRotation(newHit.point, Quaternion.LookRotation(newHit.normal, Vector3.up));
  }

  bool RaycastOpenFaces(Vector3 origin, Vector3 direction, out RaycastHit hit) {
    var didHit = Physics.Raycast(origin, direction, out hit, MaxDistance);
    var didHitBackward = didHit
      ? Physics.Raycast(hit.point - WallOffset * hit.normal, hit.normal, MaxDistance)
      : false;
    // Debug.DrawRay(origin, MaxDistance * direction, didHit && !didHitBackward ? Color.green : Color.black);
    return didHit && !didHitBackward;
  }

  List<RaycastHit> PotentialHits = new();
  RaycastHit? FindNext(RaycastHit previousHit) {
    PotentialHits.Clear();

    var normal = previousHit.normal;
    var tangent = Vector3.Cross(normal, Vector3.up);
    var position = previousHit.point + SampleSpacing * tangent;

    // let's check continuing along the path
    {
      var rayOrigin = position + WallOffset * normal;
      var rayDirection = -normal;
      if (RaycastOpenFaces(rayOrigin, rayDirection, out var hit)) {
        PotentialHits.Add(hit);
      }
    }

    // let's check an outside 90-degree corner
    {
      var rayOrigin = position - WallOffset * normal;
      var rayDirection = Quaternion.Euler(0, -90, 0) * -normal;
      if (RaycastOpenFaces(rayOrigin, rayDirection, out var hit)) {
        PotentialHits.Add(hit);
      }
    }

    // let's check an inside 90-degree corner
    {
      var rayOrigin = previousHit.point + WallOffset * normal;
      var rayDirection = Quaternion.Euler(0, 90, 0) * -normal;
      if (RaycastOpenFaces(rayOrigin, rayDirection, out var hit)) {
        PotentialHits.Add(hit);
      }
    }

    if (PotentialHits.Count > 0) {
      float bestScore = float.MinValue;
      RaycastHit hit = PotentialHits[0];
      for (var p = 0; p < PotentialHits.Count; p++)  {
        var score = Vector3.Dot(normal, PotentialHits[p].normal);
        if (score > bestScore) {
          bestScore = score;
          hit = PotentialHits[p];
        }
      }
      return hit;
    } else {
      return null;
    }
  }

  public Transform P0;
  public Transform P1;
  public Transform P2;

  void OnDrawGizmos() {
    var hit0 = new RaycastHit { point = P0.position, normal = P0.forward };
    var hit1 = new RaycastHit { point = P1.position, normal = P1.forward };
    var hit2 = new RaycastHit { point = P2.position, normal = P2.forward };
    var corners = Corners(new() { hit0, hit1, hit2 });
    corners.Insert(0, hit0);
    corners.Add(hit2);
    for (var i = 1; i < corners.Count; i++) {
      Debug.DrawLine(corners[i].point, corners[i-1].point);
    }
  }
}