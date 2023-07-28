using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WallSpaceController : MonoBehaviour {
  [SerializeField] LayerMask LayerMask;
  [SerializeField] int MaxSearchCount = 10;
  [SerializeField] float SampleSpacing = .25f;
  [SerializeField] float WallOffset = .05f;
  [SerializeField] WallEntitySegment[] RightSegments;
  [SerializeField] WallEntitySegment[] LeftSegments;
  [SerializeField] AbilityManager AbilityManager;
  #if UNITY_EDITOR
  public bool ShowHits;
  public bool ShowCorners;
  #endif
  public float Height = 1;
  public float Width = 1;
  public float MaxDistance => SampleSpacing + WallOffset;
  // An issue with this approach: the degenerate case when all sum to cancel eachother
  // out will produce a zero vector
  public Vector3 WeightedNormal {
    get {
      var normal = Vector3.zero;
      var totalWeight = 0f;
      foreach (var segment in ActiveSegments)
        totalWeight += segment.Projector.uvScale.x;
      foreach (var segment in ActiveSegments)
        normal += -segment.transform.forward * segment.Projector.uvScale.x;
      normal /= totalWeight;
      normal.Normalize();
      return normal;
    }
  }
  public UnityAction OnEnterWallSpace;
  public UnityAction OnExitWallSpace;

  List<RaycastHit> LeftCorners = new();
  List<RaycastHit> RightCorners = new();
  List<RaycastHit> RightHits = new();
  List<RaycastHit> LeftHits = new();
  List<RaycastHit> PotentialHits = new();
  IEnumerable<WallEntitySegment> ActiveSegments {
    get {
      for (var i = 0; i < RightSegments.Length; i++)
        if (RightSegments[i].gameObject.activeInHierarchy)
          yield return RightSegments[i];
      for (var i = 0; i < LeftSegments.Length; i++)
        if (LeftSegments[i].gameObject.activeInHierarchy)
          yield return LeftSegments[i];
    }
  }

  // TODO: Possibly make this more resilient to being called inappropriately?
  // Currently, it assumes there exists corner data that is valid.
  public void Move(float velocity) {
    var corners = velocity <= 0 ? LeftCorners : RightCorners;
    var pathDistance = Distance(corners);
    var distance = Mathf.Min(Mathf.Abs(velocity) * Time.fixedDeltaTime, pathDistance-Width/2);
    var newHit = Move(corners, distance);
    var p = newHit.point;
    var n = newHit.normal;
    var rTarget = Quaternion.LookRotation(n, Vector3.up);
    transform.SetPositionAndRotation(p, rTarget);
  }

  void OnEnable() {
    ActivateN(LeftSegments, 0);
    ActivateN(RightSegments, 0);
    RightHits.Clear();
    LeftHits.Clear();
    RightCorners.Clear();
    LeftCorners.Clear();
    AbilityManager.AddTag(AbilityTag.WallSpace);
    OnEnterWallSpace?.Invoke();
  }

  void OnDisable() {
    ActivateN(LeftSegments, 0);
    ActivateN(RightSegments, 0);
    RightHits.Clear();
    LeftHits.Clear();
    RightCorners.Clear();
    LeftCorners.Clear();
    AbilityManager.RemoveTag(AbilityTag.WallSpace);
    OnExitWallSpace?.Invoke();
  }

  void FixedUpdate() {
    var position = transform.position;
    var normal = transform.forward;
    var tangent = Vector3.Cross(transform.forward, Vector3.up);
    var rayOrigin = position + WallOffset * normal;
    var rayDirection = -normal;
    var didHit = Physics.Raycast(rayOrigin, rayDirection, out var hit, MaxDistance, LayerMask, QueryTriggerInteraction.Ignore);
    RightHits.Clear();
    LeftHits.Clear();
    RightCorners.Clear();
    LeftCorners.Clear();
    if (didHit) {
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

      if (RightHits.Count > 0) {
        RightCorners.Add(RightHits[0]);
        FindCorners(RightCorners, RightHits);
        RightCorners.Add(RightHits[RightHits.Count - 1]);
        RefineToPath(RightCorners);
      }
      if (LeftHits.Count > 0) {
        LeftCorners.Add(LeftHits[0]);
        FindCorners(LeftCorners, LeftHits);
        LeftCorners.Add(LeftHits[LeftHits.Count - 1]);
        RefineToPath(LeftCorners);
      }
    }
  }

  void LateUpdate() {
    // NOTE: This isn't correct or good.
    // The problem with doing this, is that the segments contain physics colliders
    // as well as decal projectors which should be updated in Fixed update.
    // When I move this code to fixed update, I see what seems to be some kind of
    // single frame hitch caused by the camera when you cross corners... it's hard to explain
    // but easy to reproduce. I don't know what is causing this though I suepect it has something
    // to do with the order the camera/WallMover are processed
    var rightSegmentCount = UpdateSegments(Width/2, RightCorners, RightSegments, right:true);
    var leftSegmentCount = UpdateSegments(Width/2, LeftCorners, LeftSegments, right:false);
    ActivateN(LeftSegments, leftSegmentCount);
    ActivateN(RightSegments, rightSegmentCount);
  }

  int FindCorners(List<RaycastHit> corners, List<RaycastHit> hits) {
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

  void RefineToPath(List<RaycastHit> corners) {
    for (var i = corners.Count-1; i > 0; i--) {
      var c0 = corners[i-1];
      var c1 = corners[i];
      if (Mathf.Approximately(Vector3.SqrMagnitude(c0.point-c1.point), 0f))
        corners.RemoveAt(i);
    }
  }

  void UpdateSegment(
  float length,
  float halfLength,
  float lengthOffset,
  Vector3 center,
  Vector3 normal,
  WallEntitySegment segment,
  bool right) {
    var min = 0f;
    var max = 0f;
    if (right) {
      min = 0.5f + lengthOffset / halfLength / 2;
      max = 0.5f + (lengthOffset + length) / halfLength / 2;
    } else {
      max = 0.5f - lengthOffset / halfLength / 2;
      min = 0.5f - (lengthOffset + length) / halfLength / 2;
    }
    segment.Projector.size = new((max-min)*halfLength*2, Height, 2*WallOffset);
    segment.Projector.uvBias = new(min, 0);
    segment.Projector.uvScale = new(max-min, 1);
    segment.Collider.size = new((max-min)*halfLength*2, Height, 2*WallOffset);
    segment.Collider.center = new(0,0,WallOffset);
    segment.transform.SetPositionAndRotation(center+WallOffset*normal, Quaternion.LookRotation(-normal, Vector3.up));
  }

  int UpdateSegments(
  float halfLength,
  List<RaycastHit> corners,
  WallEntitySegment[] segments,
  bool right) {
    var distanceOffset = 0f;
    var i = 0;
    while (i < corners.Count-1 && distanceOffset < halfLength) {
      var c0 = corners[i];
      var c1 = corners[i+1];
      var delta = c1.point-c0.point;
      var cornerDistance = delta.magnitude;
      var direction = delta / cornerDistance;
      var distance = Mathf.Min(halfLength-distanceOffset, cornerDistance);
      var start = c0.point;
      var end = start+distance*direction;
      var center = start+(end-start) / 2;
      var normal = Vector3.Cross(right ? -direction : direction, Vector3.up);
      UpdateSegment(distance, halfLength, distanceOffset, center, normal, segments[i], right);
      distanceOffset += distance;
      i++;
    }
    return i;
  }

  void ActivateN<T>(T[] ts, int count) where T : MonoBehaviour {
    for (var i = 0; i < ts.Length; i++) {
      ts[i].gameObject.SetActive(i<count);
    }
  }

  bool LineLineIntersection(
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

  RaycastHit Move(List<RaycastHit> corners, float distance) {
    for (var i = 1; i < corners.Count; i++) {
      var p0 = corners[i-1].point;
      var p1 = corners[i].point;
      var d = Vector3.Distance(p0, p1);
      if (d >= distance) {
        var np = Vector3.Lerp(p0, p1, distance / d);
        return new RaycastHit() { point = np, normal = corners[i].normal };
      } else {
        distance -= d;
      }
    }
    return corners[corners.Count - 1];
  }

  float Distance(List<RaycastHit> hits, float distance = 0) {
    for (var i = 1; i < hits.Count; i++)
      distance += Vector3.Distance(hits[i].point, hits[i-1].point);
    return distance;
  }

  bool RaycastOpenFaces(Vector3 origin, Vector3 direction, out RaycastHit hit) {
    if (Physics.Raycast(origin, direction, out hit, MaxDistance, LayerMask, QueryTriggerInteraction.Ignore)) {
      var didHitBackward = Physics.Raycast(hit.point - WallOffset * hit.normal, hit.normal, MaxDistance, LayerMask, QueryTriggerInteraction.Ignore);
      var didHitBlocker = hit.collider.GetComponent<Blocker>();
      return !didHitBackward && !didHitBlocker;
    }
    return false;
  }

  RaycastHit? FindNext(RaycastHit previousHit, float sign = 1) {
    PotentialHits.Clear();

    var normal = previousHit.normal;
    var tangent = sign * Vector3.Cross(normal, Vector3.up);
    var position = previousHit.point + SampleSpacing * tangent;

    {
      var rayOrigin = position + WallOffset * normal;
      var rayDirection = -normal;
      if (RaycastOpenFaces(rayOrigin, rayDirection, out var hit)) {
        PotentialHits.Add(hit);
      }
    }

    {
      var rayOrigin = position - WallOffset * normal;
      var rayDirection = Quaternion.Euler(0, -sign * 90, 0) * -normal;
      if (RaycastOpenFaces(rayOrigin, rayDirection, out var hit)) {
        PotentialHits.Add(hit);
      }
    }

    {
      var rayOrigin = previousHit.point + WallOffset * normal;
      var rayDirection = Quaternion.Euler(0, sign * 90, 0) * -normal;
      if (RaycastOpenFaces(rayOrigin, rayDirection, out var hit)) {
        PotentialHits.Add(hit);
      }
    }

    if (PotentialHits.Count > 0) {
      var bestScore = float.MinValue;
      var hit = PotentialHits[0];
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

  #if UNITY_EDITOR
  void OnDrawGizmos() {
    if (ShowHits) {
      RenderHits(RightHits);
      RenderHits(LeftHits);
    }
    if (ShowCorners) {
      RenderCorners(RightCorners);
      RenderCorners(LeftCorners);
    }
  }
  #endif
}