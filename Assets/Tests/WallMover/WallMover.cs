using System;
using System.Collections.Generic;
using UnityEngine;

public class WallMover : MonoBehaviour {
  [Tooltip("Max number of checks")]
  public int MaxSearchCount = 10;
  [Tooltip("Distance along the wall to check for next contact")]
  public float SampleSpacing = .25f;
  [Tooltip("Distance from wall to cast rays from")]
  public float Height = 1;
  public float Width = 1;
  public float WallOffset = .05f;
  public float MaxDistance => SampleSpacing + WallOffset;
  public Collider Collider;
  public float Speed = 2;
  public WallSegment[] RightWallSegments;
  public WallSegment[] LeftWallSegments;

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

  public static void ConfigureSegment(
  float length,
  float totalDistance,
  float distanceOffset,
  Vector3 center,
  Vector3 normal,
  WallSegment segment,
  bool right) {
    // Steve: Magic 2s are symptoms of the totalDistance being half the width of the actual image
    const float WALL_OFFSET = .1f;
    segment.transform.SetPositionAndRotation(center+WALL_OFFSET*normal, Quaternion.LookRotation(-normal, Vector3.up));
    segment.Width = totalDistance * 2;
    segment.Height = 1;
    segment.Depth = 2*WALL_OFFSET;
    if (right) {
      segment.Min = 0.5f + distanceOffset / totalDistance / 2;
      segment.Max = 0.5f + (distanceOffset + length) / totalDistance / 2;
    } else {
      segment.Max = 0.5f - distanceOffset / totalDistance / 2;
      segment.Min = 0.5f - (distanceOffset + length) / totalDistance / 2;
    }
  }

  // returns number of segments used
  public static int ConfigureSegments(
  float totalDistance,
  List<RaycastHit> corners,
  WallSegment[] wallSegments,
  bool right) {
    var distanceOffset = 0f;
    var i = 0;
    while (i < corners.Count-1 && distanceOffset < totalDistance) {
      var c0 = corners[i];
      var c1 = corners[i+1];
      var delta = c1.point-c0.point;
      var cornerDistance = delta.magnitude;
      var direction = delta / cornerDistance;
      var distance = Mathf.Min(totalDistance-distanceOffset, cornerDistance);
      var start = c0.point;
      var end = start+distance*direction;
      var center = start+(end-start) / 2;
      var normal = Vector3.Cross(right ? -direction : direction, Vector3.up);
      ConfigureSegment(distance, totalDistance, distanceOffset, center, normal, wallSegments[i], right);
      distanceOffset += distance;
      i++;
    }
    return i;
  }

  public static void ActivateN<T>(T[] ts, int count) where T : MonoBehaviour {
    for (var i = 0; i < count; i++) {
      ts[i].gameObject.SetActive(i<count);
    }
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

  public static float Distance(List<RaycastHit> hits, float distance = 0) {
    for (var i = 1; i < hits.Count; i++) {
      distance += Vector3.Distance(hits[i].point, hits[i-1].point);
    }
    return distance;
  }

  List<RaycastHit> RightHits = new();
  List<RaycastHit> LeftHits = new();
  void FixedUpdate() {
    var position = transform.position;
    var normal = transform.forward;
    var tangent = Vector3.Cross(transform.forward, Vector3.up);
    var rayOrigin = position + WallOffset * normal;
    var rayDirection = -normal;
    var didHit = Physics.Raycast(rayOrigin, rayDirection, out var hit, MaxDistance);
    if (didHit) {
      if (hit.collider != Collider) {
        transform.SetParent(hit.collider.transform, true);
        Collider = hit.collider;
      }

      // right tracing
      RightHits.Clear();
      RightHits.Add(hit);
      var rightHit = hit;
      for (var i = 0; i < MaxSearchCount; i++) {
        var nextHit = FindNext(rightHit);
        if (nextHit.HasValue) {
          RightHits.Add(nextHit.Value);
          rightHit = nextHit.Value;
        } else {
          break;
        }
      }

      // left tracing
      LeftHits.Clear();
      LeftHits.Add(hit);
      var leftHit = hit;
      for (var i = 0; i < MaxSearchCount; i++) {
        var nextHit = FindNext(leftHit, sign:-1);
        if (nextHit.HasValue) {
          LeftHits.Add(nextHit.Value);
          leftHit = nextHit.Value;
        } else {
          break;
        }
      }
    }

    // calculate corners
    var rightCorners = Corners(RightHits);
    rightCorners.Insert(0, RightHits[0]);
    rightCorners.Add(RightHits[RightHits.Count - 1]);
    var leftCorners = Corners(LeftHits);
    leftCorners.Insert(0, LeftHits[0]);
    leftCorners.Add(LeftHits[LeftHits.Count - 1]);

    // compute movement
    var pathDistance = Distance(rightCorners);
    var distance = Mathf.Min(Speed * Time.fixedDeltaTime, pathDistance);
    var newHit = Move(rightCorners, distance);
    var p = newHit.point;
    var n = newHit.normal;
    var rTarget = Quaternion.LookRotation(n, Vector3.up);
    transform.SetPositionAndRotation(p, rTarget);

    // Move segments after the parent
    var rightSegmentCount = ConfigureSegments(Width/2, rightCorners, RightWallSegments, right:true);
    var leftSegmentCount = ConfigureSegments(Width/2, leftCorners, LeftWallSegments, right:false);
    ActivateN(LeftWallSegments, leftSegmentCount);
    ActivateN(RightWallSegments, rightSegmentCount);
  }

  bool RaycastOpenFaces(Vector3 origin, Vector3 direction, out RaycastHit hit) {
    var didHit = Physics.Raycast(origin, direction, out hit, MaxDistance);
    var didHitBackward = didHit
      ? Physics.Raycast(hit.point - WallOffset * hit.normal, hit.normal, MaxDistance)
      : false;
    return didHit && !didHitBackward;
  }

  List<RaycastHit> PotentialHits = new();
  RaycastHit? FindNext(RaycastHit previousHit, float sign = 1) {
    PotentialHits.Clear();

    var normal = previousHit.normal;
    var tangent = sign * Vector3.Cross(normal, Vector3.up);
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
      var rayDirection = Quaternion.Euler(0, -sign * 90, 0) * -normal;
      if (RaycastOpenFaces(rayOrigin, rayDirection, out var hit)) {
        PotentialHits.Add(hit);
      }
    }

    // let's check an inside 90-degree corner
    {
      var rayOrigin = previousHit.point + WallOffset * normal;
      var rayDirection = Quaternion.Euler(0, sign * 90, 0) * -normal;
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