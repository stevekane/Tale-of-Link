using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using KinematicCharacterController;

[DefaultExecutionOrder(3)]
[RequireComponent(typeof(LocalTime))]
[RequireComponent(typeof(AbilityManager))]
public class WallSpaceController : MonoBehaviour {
  [SerializeField] LayerMask LayerMask;
  [SerializeField] int MaxSearchCount = 10;
  [SerializeField] float SampleSpacing = .25f;
  [SerializeField] float WallOffset = .05f;
  [SerializeField] float SegmentDepth = .02f;
  [SerializeField] float SegmentOffset = .01f;
  [SerializeField] WallEntitySegment[] RightSegments;
  [SerializeField] WallEntitySegment[] LeftSegments;
  public MovingWall MovingWall;
  #if UNITY_EDITOR
  public bool ShowHits;
  public bool ShowPath;
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
        totalWeight += segment.transform.localScale.x;
      foreach (var segment in ActiveSegments)
        normal += -segment.transform.forward * segment.transform.localScale.x;
      normal /= totalWeight;
      normal.Normalize();
      return normal == Vector3.zero ? transform.forward : normal;
    }
  }
  public UnityAction OnEnterWallSpace;
  public UnityAction OnExitWallSpace;

  LocalTime LocalTime;
  AbilityManager AbilityManager;
  List<RaycastHit> LeftPath = new();
  List<RaycastHit> RightPath = new();
  List<RaycastHit> RightHits = new();
  List<RaycastHit> LeftHits = new();
  List<(RaycastHit, bool)> PotentialHits = new();
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

  float Velocity;
  public void Move(float velocity) {
    Velocity = velocity;
  }

  bool MergeRequested;
  Vector3 MergePosition;
  Vector3 MergeForward;
  public void Merge(Vector3 position, Vector3 forward) {
    MergeRequested = true;
    MergePosition = position;
    MergeForward = forward;
  }

  void Awake() {
    this.InitComponent(out AbilityManager);
    this.InitComponent(out LocalTime);
  }

  void OnEnable() {
    RightHits.Clear();
    LeftHits.Clear();
    RightPath.Clear();
    LeftPath.Clear();
    AbilityManager.AddTag(AbilityTag.WallSpace);
    OnEnterWallSpace?.Invoke();
  }

  void OnDisable() {
    RightHits.Clear();
    LeftHits.Clear();
    RightPath.Clear();
    LeftPath.Clear();
    AbilityManager.RemoveTag(AbilityTag.WallSpace);
    OnExitWallSpace?.Invoke();
  }

  void FixedUpdate() {
    if (MergeRequested) {
      transform.position = MergePosition + (MovingWall ? MovingWall.PreviousMotionDelta : Vector3.zero);
      transform.rotation = Quaternion.LookRotation(MergeForward, Vector3.up);
      MergeRequested = false;
    }

    var position = transform.position + transform.forward * WallOffset;
    var direction = -transform.forward;
    var didHit = Physics.Raycast(position, direction, out var firstHit, 2*WallOffset, LayerMask, QueryTriggerInteraction.Ignore);
    MovingWall = didHit ? firstHit.collider.GetComponent<MovingWall>() : null;
    if (didHit) {
      var ignoreBackFaces = Physics.Raycast(firstHit.point - WallOffset * firstHit.normal, firstHit.normal, MaxDistance, LayerMask, QueryTriggerInteraction.Ignore);
      UpdateHits(RightHits, firstHit, 1, ignoreBackFaces);
      UpdateHits(LeftHits, firstHit, -1, ignoreBackFaces);
      UpdatePath(RightPath, RightHits);
      UpdatePath(LeftPath, LeftHits);
      MovePath(RightPath, MovingWall);
      MovePath(LeftPath, MovingWall);
    }

    var path = Velocity <= 0 ? LeftPath : RightPath;
    if (path.Count > 0) {
      var pathDistance = Distance(path);
      var distance = Mathf.Min(Mathf.Abs(Velocity) * LocalTime.FixedDeltaTime, pathDistance-Width/2);
      var newHit = PathMove(path, distance);
      var p = newHit.point;
      var n = newHit.normal;
      var rTarget = Quaternion.LookRotation(n, Vector3.up);
      transform.SetPositionAndRotation(p, rTarget);
    }
    Velocity = 0;

    UpdateSegments(Width/2, RightPath, RightSegments, right:true);
    UpdateSegments(Width/2, LeftPath, LeftSegments, right:false);
  }

  void UpdateHits(List<RaycastHit> hits, RaycastHit firstHit, float sign, bool ignoreBackFaces) {
    hits.Clear();
    hits.Add(firstHit);
    var rightHit = firstHit;
    var rightIgnoreBackFaces = ignoreBackFaces;
    for (var i = 0; i < MaxSearchCount; i++) {
      var nextHit = FindNext(rightHit, sign, ref rightIgnoreBackFaces);
      if (nextHit.HasValue) {
        hits.Add(nextHit.Value);
        rightHit = nextHit.Value;
      } else {
        break;
      }
    }
  }

  void UpdatePath(List<RaycastHit> path, List<RaycastHit> hits) {
    path.Clear();
    path.Add(hits[0]);
    for (int i = 0; i < hits.Count - 1; i++) {
      Vector3 currentPoint = hits[i].point;
      Vector3 currentNormal = hits[i].normal;
      Vector3 currentTangent = Vector3.Cross(currentNormal, Vector3.up);
      Vector3 nextPoint = hits[i + 1].point;
      Vector3 nextNormal = hits[i + 1].normal;
      Vector3 nextTangent = Vector3.Cross(nextNormal, Vector3.up);
      if (LineLineIntersection(out var point, currentPoint, currentTangent, nextPoint, nextTangent)) {
        path.Add(new RaycastHit() { point = point, normal = currentNormal });
      }
    }
    path.Add(hits[^1]);
    for (var i = path.Count-1; i > 0; i--) {
      var p0 = path[i-1];
      var p1 = path[i];
      if (Mathf.Approximately(Vector3.SqrMagnitude(p0.point-p1.point), 0f))
        path.RemoveAt(i);
    }
  }

  void MovePath(List<RaycastHit> path, MovingWall movingWall) {
    // Steve : these corners are VALUE types which makes updating them deceptively tricky
    if (movingWall) {
      for (var i = 0; i < path.Count; i++) {
        var p = path[i];
        p.point += movingWall.MotionDelta;
        path[i] = p;
      }
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
    segment.MeshRenderer.material.SetVector("_BaseMap_ST", new Vector4(max-min, 1, min, 0));
    segment.transform.localScale = new(Width * (max-min), Height, SegmentDepth);
    segment.transform.SetPositionAndRotation(center+SegmentOffset*normal, Quaternion.LookRotation(-normal, Vector3.up));
  }

  int UpdateSegments(
  float halfLength,
  List<RaycastHit> corners,
  WallEntitySegment[] segments,
  bool right) {
    var distanceOffset = 0f;
    var activeCount = 0;
    while (activeCount < corners.Count-1 && distanceOffset < halfLength) {
      var c0 = corners[activeCount];
      var c1 = corners[activeCount+1];
      var delta = c1.point-c0.point;
      var cornerDistance = delta.magnitude;
      var direction = delta / cornerDistance;
      var distance = Mathf.Min(halfLength-distanceOffset, cornerDistance);
      var start = c0.point;
      var end = start+distance*direction;
      var center = start+(end-start) / 2;
      var normal = Vector3.Cross(right ? -direction : direction, Vector3.up);
      UpdateSegment(distance, halfLength, distanceOffset, center, normal, segments[activeCount], right);
      distanceOffset += distance;
      activeCount++;
    }
    for (var i = 0; i < segments.Length; i++) {
      segments[i].gameObject.SetActive(i < activeCount);
    }
    return activeCount;
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

  RaycastHit PathMove(List<RaycastHit> corners, float distance) {
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
    return corners[^1];
  }

  float Distance(List<RaycastHit> hits, float distance = 0) {
    for (var i = 1; i < hits.Count; i++)
      distance += Vector3.Distance(hits[i].point, hits[i-1].point);
    return distance;
  }

  bool RaycastOpenFaces(
  Vector3 origin,
  Vector3 direction,
  out RaycastHit hit,
  out bool hitBackFace,
  ref bool ignoreBackFaces) {
    if (Physics.Raycast(origin, direction, out hit, MaxDistance, LayerMask, QueryTriggerInteraction.Ignore)) {
      hitBackFace = Physics.Raycast(hit.point - WallOffset * hit.normal, hit.normal, out var backHit, MaxDistance, LayerMask, QueryTriggerInteraction.Ignore);
      // hit a blocker
      if (hit.collider.GetComponent<Blocker>()) {
        DrawArrow(origin, MaxDistance * direction, Color.red, .25f * MaxDistance);
        return false;
      // did not hit a backface
      } else if (hitBackFace) {
        // we hit a backface but one of the two objects moves or the backface hit is a blocker (need to get free)
        if (ignoreBackFaces && (backHit.collider.GetComponent<Blocker>() || (backHit.collider.GetComponent<PhysicsMover>() || hit.collider.GetComponent<PhysicsMover>()))) {
          return true;
        // hit a bad backface
        } else {
          DrawArrow(origin, MaxDistance * direction, Color.blue, .25f * MaxDistance);
          return false;
        }
      // valid
      } else {
        DrawArrow(origin, MaxDistance * direction, Color.green, .25f * MaxDistance);
        return true;
      }
    }
    DrawArrow(origin, MaxDistance * direction, Color.black, .25f * MaxDistance);
    hitBackFace = false;
    return false;
  }

  public static void DrawArrow(Vector3 start, Vector3 dir, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
    Vector3 right = Quaternion.LookRotation(dir) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
    Vector3 left = Quaternion.LookRotation(dir) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
    Debug.DrawRay(start, dir, color);
    Debug.DrawRay(start + dir, right * arrowHeadLength, color);
    Debug.DrawRay(start + dir, left * arrowHeadLength, color);
  }

  RaycastHit? FindNext(RaycastHit previousHit, float sign, ref bool ignoreBackFaces) {
    PotentialHits.Clear();

    var n0 = previousHit.normal;
    var t0 = sign * Vector3.Cross(n0, Vector3.up);
    var p0 = previousHit.point;
    var p1 = p0 + SampleSpacing * t0;

    const float MIN_THETA = -90;
    const float MAX_THETA = 90;
    const float D_THETA = 30;
    for (var theta = MIN_THETA; theta <= MAX_THETA; theta += D_THETA) {
      var rotation = Quaternion.Euler(0, theta, 0);
      var n1 = rotation * n0;
      var t1 = rotation * t0;
      var pfar = p1 + SampleSpacing * t1 + WallOffset * n1;
      var pnear = p0 + SampleSpacing * t1 + WallOffset * n1;
      var hit = new RaycastHit();
      var hitBackFace = false;
      if (RaycastOpenFaces(pfar, -n1, out hit, out hitBackFace, ref ignoreBackFaces)) {
        PotentialHits.Add((hit, hitBackFace));
      }
      if (RaycastOpenFaces(pnear, -n1, out hit, out hitBackFace, ref ignoreBackFaces)) {
        PotentialHits.Add((hit, hitBackFace));
      }
    }

    if (PotentialHits.Count > 0) {
      var bestScore = float.MinValue;
      var hit = PotentialHits[0].Item1;
      var didHitBackface = false;
      for (var p = 0; p < PotentialHits.Count; p++)  {
        var score = Vector3.Dot(n0, PotentialHits[p].Item1.normal);
        if (score > bestScore) {
          bestScore = score;
          hit = PotentialHits[p].Item1;
          didHitBackface = PotentialHits[p].Item2;
        }
      }
      ignoreBackFaces = didHitBackface;
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

  void RenderPath(List<RaycastHit> path) {
    Gizmos.color = Color.white;
    for (var i = 1; i < path.Count; i++) {
      Gizmos.DrawLine(path[i].point, path[i-1].point);
    }
    foreach (var p in path) {
      Gizmos.DrawWireSphere(p.point, .1f);
    }
  }

  #if UNITY_EDITOR
  void OnDrawGizmos() {
    if (ShowHits) {
      RenderHits(RightHits);
      RenderHits(LeftHits);
    }
    if (ShowPath) {
      RenderPath(RightPath);
      RenderPath(LeftPath);
    }
  }
  #endif
}