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
  public Collider Collider;
  public float Speed = 2;
  public WallSegment[] WallSegments;

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

  public static float Distance(List<RaycastHit> hits, float distance = 0) {
    for (var i = 1; i < hits.Count; i++) {
      distance += Vector3.Distance(hits[i].point, hits[i-1].point);
    }
    return distance;
  }

  void Start() {
    Debug.Log(Distance(new()));
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
    Debug.DrawRay(rayOrigin, MaxDistance * rayDirection, didHit ? Color.green : Color.black);
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
    /*
    Next steps:

    Trace both forward and backward paths and corners
    March along the backward corners by half your width
      For every corner encountered, measure the distance of that strip
      and assign a segment
    March along the forward corners by half your width
      For every corner encountered, measure the distance of that strip
      and assign a segment
    */

    // right corner calcs
    var rightCorners = Corners(RightHits);
    rightCorners.Insert(0, RightHits[0]);
    rightCorners.Add(RightHits[RightHits.Count - 1]);

    // left corner calcs
    var leftCorners = Corners(LeftHits);
    leftCorners.Insert(0, LeftHits[0]);
    leftCorners.Add(RightHits[LeftHits.Count - 1]);

    var pathDistance = Distance(rightCorners);
    var distance = Mathf.Min(Speed * Time.fixedDeltaTime, pathDistance);
    for (var i = 1; i < rightCorners.Count; i++) {
      Debug.DrawLine(rightCorners[i].point, rightCorners[i-1].point);
    }
    var newHit = Move(rightCorners, distance);
    var p = newHit.point;
    var n = newHit.normal;
    var rTarget = Quaternion.LookRotation(n, Vector3.up);
    const float WIDTH = 1; // TODO: make param
    const float WALL_OFFSET = .1f; // TODO: make param
    var firstSegmentLocalPosition = -WIDTH/2 * Vector3.left;
    var segmentSpacing = -WIDTH/WallSegments.Length;
    for (var i = 0; i < WallSegments.Length; i++) {
      var segment = WallSegments[i];
      var direction = Quaternion.LookRotation(-n, Vector3.up);
      segment.transform.localPosition = firstSegmentLocalPosition + i*segmentSpacing*Vector3.right + WALL_OFFSET*Vector3.forward;
      segment.Width = 1;
      segment.Height = 1;
      segment.Depth = 1;
      segment.Min = (float)i/WallSegments.Length;
      segment.Max = (float)(i+1)/WallSegments.Length;
      segment.transform.rotation = direction;
    }
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

  void OnDrawGizmos() {
    foreach (var hit in RightHits) {
      Debug.DrawRay(hit.point, hit.normal, Color.green);
    }
    foreach (var hit in LeftHits) {
      Debug.DrawRay(hit.point, hit.normal, Color.yellow);
    }
  }
}