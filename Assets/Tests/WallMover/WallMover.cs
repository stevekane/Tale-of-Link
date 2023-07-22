using System.Collections.Generic;
using UnityEngine;

public class WallMover : MonoBehaviour {
  public LayerMask LayerMask;
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
  public WallSegment[] RightWallSegments;
  public WallSegment[] LeftWallSegments;
  public float Velocity;

  #if UNITY_EDITOR
  public bool ShowHits;
  public bool ShowCorners;
  public bool ShowSegments;
  #endif

  List<RaycastHit> LeftCorners = new();
  List<RaycastHit> RightCorners = new();

  public static int FindCorners(List<RaycastHit> corners, List<RaycastHit> hits) {
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
    return corners.Count;
  }

  /*
  Requirements:

    + Move robustly in either direction
    + Handle entering wall space
    + Handle exiting wall space
    - Encounter obstacles that prevent you from passing
    - Handle case where you cannot move as much as you want to
    - Handle case where you cannot render as much as you want to
    - Handle case of moving onto a moving wall
    - Handle case of moving wall moving away
    - Handle collision detection with other wall items
  */

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
    for (var i = 0; i < ts.Length; i++) {
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

  IEnumerable<WallSegment> ActiveSegments {
    get {
      for (var i = 0; i < RightWallSegments.Length; i++)
        if (RightWallSegments[i].isActiveAndEnabled)
          yield return RightWallSegments[i];
      for (var i = 0; i < LeftWallSegments.Length; i++)
        if (LeftWallSegments[i].isActiveAndEnabled)
          yield return LeftWallSegments[i];
    }
  }

  // returns the normal of the active segments weighted by its width
  public Vector3 WeightedNormal {
    get {
      var normal = Vector3.zero;
      var totalWeight = 0f;
      foreach (var segment in ActiveSegments)
        totalWeight += segment.Width;
      foreach (var segment in ActiveSegments)
        normal += -segment.transform.forward * (segment.Max-segment.Min);
      normal /= totalWeight;
      normal.Normalize();
      return normal;
    }
  }

  void OnDisable() {
    ActivateN(LeftWallSegments, 0);
    ActivateN(RightWallSegments, 0);
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
        // transform.SetParent(hit.collider.transform, true);
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
    RightCorners.Clear();
    RightCorners.Add(RightHits[0]);
    FindCorners(RightCorners, RightHits);
    RightCorners.Add(RightHits[RightHits.Count - 1]);
    LeftCorners.Clear();
    LeftCorners.Add(LeftHits[0]);
    FindCorners(LeftCorners, LeftHits);
    LeftCorners.Add(LeftHits[LeftHits.Count - 1]);

    // compute movement
    var corners = Velocity <= 0 ? LeftCorners : RightCorners;
    var pathDistance = Distance(corners);
    var distance = Mathf.Min(Mathf.Abs(Velocity) * Time.fixedDeltaTime, pathDistance);
    var newHit = Move(corners, distance);
    var p = newHit.point;
    var n = newHit.normal;
    var rTarget = Quaternion.LookRotation(n, Vector3.up);
    transform.SetPositionAndRotation(p, rTarget);

    // Move segments after the parent
    var rightSegmentCount = ConfigureSegments(Width/2, RightCorners, RightWallSegments, right:true);
    var leftSegmentCount = ConfigureSegments(Width/2, LeftCorners, LeftWallSegments, right:false);
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

  void RenderHits(List<RaycastHit> hits) {
    Gizmos.color = Color.grey;
    foreach (var hit in hits) {
      Gizmos.DrawRay(hit.point, hit.normal);
    }
  }

  void RenderCorners(List<RaycastHit> corners) {
    Gizmos.color = Color.white;
    for (var i = 1; i < corners.Count; i++) {
      Gizmos.DrawLine(corners[i].point, corners[i-1].point);
    }
    foreach (var corner in corners) {
      Gizmos.DrawWireSphere(corner.point, .25f);
    }
  }

  void RenderSegments(WallSegment[] segments) {
    foreach (var segment in segments) {
      if (!segment.isActiveAndEnabled)
        continue;
      var offset = segment.Width * (segment.Max-segment.Min) * .5f * segment.transform.right;
      var start = segment.transform.position + offset;
      var end = segment.transform.position - offset;
      Gizmos.color = segment.Color;
      Gizmos.DrawLine(start, end);
    }
  }

  void OnDrawGizmos() {
    if (ShowHits) {
      RenderHits(RightHits);
      RenderHits(LeftHits);
    }
    if (ShowCorners) {
      RenderCorners(RightCorners);
      RenderCorners(LeftCorners);
    }
    if (ShowSegments) {
      RenderSegments(RightWallSegments);
      RenderSegments(LeftWallSegments);
    }
  }
}