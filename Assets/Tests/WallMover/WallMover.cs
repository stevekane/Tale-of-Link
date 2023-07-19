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
  public float Speed = 2;
  public Transform P0;
  public Transform P1;
  public Transform P2;
  public Transform Wanderer;
  float DistanceTraveled;

  public static List<RaycastHit> Corners(List<RaycastHit> hits) {
    List<RaycastHit> corners = new List<RaycastHit>();
    for (int i = 0; i < hits.Count - 1; i++) {
      Vector3 currentPoint = hits[i].point;
      Vector3 currentNormal = hits[i].normal;
      Vector3 currentTangent = Vector3.Cross(currentNormal, Vector3.up);
      Vector3 nextPoint = hits[i + 1].point;
      Vector3 nextNormal = hits[i + 1].normal;
      Vector3 nextTangent = Vector3.Cross(nextNormal, Vector3.up);
      if (LineLineIntersection(out var corner, currentPoint, currentTangent, nextPoint, nextTangent)) {
        corners.Add(new RaycastHit() { point = corner, normal = currentNormal });
      }
    }
    return corners;
  }

  public static bool LineLineIntersection(
  out Vector3 intersection,
  Vector3 linePoint1,
  Vector3 lineDirection1,
  Vector3 linePoint2,
  Vector3 lineDirection2) {
    var lineVec3 = linePoint2 - linePoint1;
    var crossVec1and2 = Vector3.Cross(lineDirection1, lineDirection2);
    var crossVec3and2 = Vector3.Cross(lineVec3, lineDirection2);
    var planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

    if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f) {
      var s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
      intersection = linePoint1 + (lineDirection1 * s);
      return true;
    } else {
      intersection = Vector3.zero;
      return false;
    }
  }

  public static RaycastHit Move(List<RaycastHit> hits, float distance) {
    for (var i = 1; i < hits.Count; i++) {
      var p0 = hits[i-1].point;
      var p1 = hits[i].point;
      var d = Vector3.Distance(p0, p1);
      if (d >= distance) {
        var np = Vector3.Lerp(p0, p1, distance / d);
        return new RaycastHit() { point = np, normal = hits[i].normal };
      } else {
        distance -= d;
      }
    }
    return hits[hits.Count - 1];
  }

  public static float Distance(List<RaycastHit> hits) {
    var distance = 0f;
    if (hits.Count <= 1)
      return distance;

    for (var i = 1; i < hits.Count; i++) {
      distance += Vector3.Distance(hits[i].point, hits[i-1].point);
    }
    return distance;
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
      if (hit.collider != Collider) {
        transform.SetParent(hit.collider.transform, true);
        Collider = hit.collider;
      }
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
    var corners = Corners(Hits);
    corners.Insert(0, Hits[0]);
    corners.Add(Hits[Hits.Count - 1]);
    var pathDistance = Distance(corners);
    var distance = Mathf.Min(Speed * Time.fixedDeltaTime, pathDistance);
    for (var i = 1; i < corners.Count; i++) {
      Debug.DrawLine(corners[i].point, corners[i-1].point);
    }
    DistanceTraveled += distance;
    var newHit = Move(corners, distance);
    var p = newHit.point;
    var n = newHit.normal;
    // TODO: Must account for there not being a next corner (I think this is always technically possible?)
    var distanceToNextCorner = Vector3.Distance(corners[0].point, corners[1].point);
    var distanceToStartTurning = 1f;
    var interpolant = Mathf.Clamp01(distanceToNextCorner / distanceToStartTurning);
    var forward = Vector3.Lerp(corners[2].normal, corners[1].normal, interpolant); // very weird. looking ahead two corners here because of the way normals are stored currently
    Debug.DrawRay(corners[0].point, corners[0].normal, Color.cyan);
    Debug.DrawRay(corners[1].point, corners[1].normal, Color.yellow);
    Debug.Log($"DISTANCE: {distanceToNextCorner} | INTERPOLANT: {interpolant} | FORWARD: {forward}");
    // var rTarget = Quaternion.LookRotation(n, Vector3.up);
    var rTarget = Quaternion.LookRotation(forward, Vector3.up);
    // var r = Quaternion.RotateTowards(transform.rotation, rTarget, 180 * Time.fixedDeltaTime);
    transform.SetPositionAndRotation(p, rTarget);
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
}