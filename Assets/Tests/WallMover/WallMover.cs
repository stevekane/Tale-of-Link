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

  // returns number of segments used
  public static int ConfigureSegments(float distance, List<RaycastHit> corners, WallSegment[] wallSegments) {
    const float WALL_OFFSET = .1f;
    int segments = 0;
    float remainingDistance = distance;
    for (var i = 1; i < corners.Count; i++) {
      var c0 = corners[i-1];
      var c1 = corners[i];
      var delta = c1.point-c0.point;
      var direction = delta.normalized;
      var d = Vector3.Distance(c0.point, c1.point);
      if (d >= remainingDistance) {
        // create final segment
        var segment = wallSegments[segments];
        segment.transform.position = c0.point + direction * remainingDistance / 2 + WALL_OFFSET * c0.normal;
        segment.transform.rotation = Quaternion.LookRotation(-c0.normal, Vector3.up);
        segment.Width = distance * 2;
        segment.Height = 1;
        segment.Depth = 1;
        // TODO: Correct Min/Max
        segment.Min = 0;
        segment.Max = 1;
        Debug.DrawLine(c0.point, c0.point + remainingDistance*direction, segment.Color);
        segments += 1;
        break;
      } else {
        // create segment and continue
        var segment = wallSegments[segments];
        segment.transform.position = delta / 2 + WALL_OFFSET * c0.normal;
        segment.transform.rotation = Quaternion.LookRotation(-c0.normal, Vector3.up);
        segment.Width = distance * 2;
        segment.Height = 1;
        segment.Depth = 1;
        // TODO: Correct Min/Max
        segment.Min = 0;
        segment.Max = 1;
        Debug.DrawLine(c0.point, c1.point, segment.Color);
        remainingDistance -= d;
        segments += 1;
      }
    }
    return segments;
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
    var rightSegments = ConfigureSegments(.5f, rightCorners, RightWallSegments);
    for (var i = 0; i < RightWallSegments.Length; i++) {
      RightWallSegments[i].gameObject.SetActive(i < rightSegments);
    }

    // left corner calcs
    var leftCorners = Corners(LeftHits);
    leftCorners.Insert(0, LeftHits[0]);
    leftCorners.Add(LeftHits[LeftHits.Count - 1]);
    var leftSegments = ConfigureSegments(.5f, leftCorners, LeftWallSegments);
    for (var i = 0; i < LeftWallSegments.Length; i++) {
      LeftWallSegments[i].gameObject.SetActive(i < leftSegments);
    }

    // foreach (var corner in rightCorners) {
    //   Debug.DrawRay(corner.point, corner.normal);
    // }
    // foreach (var corner in leftCorners) {
    //   Debug.DrawRay(corner.point, corner.normal);
    // }
    // for (var i = 1; i < rightCorners.Count; i++) {
    //   Debug.DrawLine(rightCorners[i].point, rightCorners[i-1].point);
    // }
    // for (var i = 1; i < leftCorners.Count; i++) {
    //   Debug.DrawLine(leftCorners[i].point, leftCorners[i-1].point);
    // }

    var pathDistance = Distance(rightCorners);
    var distance = Mathf.Min(Speed * Time.fixedDeltaTime, pathDistance);
    var newHit = Move(rightCorners, distance);
    var p = newHit.point;
    var n = newHit.normal;
    var rTarget = Quaternion.LookRotation(n, Vector3.up);
    const float WIDTH = 1; // TODO: make param
    const float WALL_OFFSET = .1f; // TODO: make param
    var firstSegmentLocalPosition = -WIDTH/2 * Vector3.left;
    var segmentSpacing = -WIDTH/RightWallSegments.Length;
    for (var i = 0; i < RightWallSegments.Length; i++) {
      var segment = RightWallSegments[i];
      var direction = Quaternion.LookRotation(-n, Vector3.up);
      segment.transform.localPosition = firstSegmentLocalPosition + i*segmentSpacing*Vector3.right + WALL_OFFSET*Vector3.forward;
      segment.Width = 1;
      segment.Height = 1;
      segment.Depth = 1;
      segment.Min = (float)i/RightWallSegments.Length;
      segment.Max = (float)(i+1)/RightWallSegments.Length;
      segment.transform.rotation = direction;
    }
    transform.SetPositionAndRotation(p, rTarget);
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