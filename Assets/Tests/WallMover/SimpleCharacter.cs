using System.Collections;
using UnityEngine;
using Cinemachine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[DefaultExecutionOrder(-700)]
public class SimpleCharacter : MonoBehaviour {
  [SerializeField] Timeval WallTransitionDuration;
  [SerializeField] GameObject Model;
  [SerializeField] float GroundSpeed = 5;
  [SerializeField] float WallSpeed = 3;
  [SerializeField] CinemachineVirtualCamera TopdownCamera;
  [SerializeField] CinemachineVirtualCamera WallCamera;
  [SerializeField] CapsuleCollider CapsuleCollider;
  [SerializeField] Mesh CapsuleMesh;

  Inputs Inputs;
  WallMover WallMover;
  bool InTransition;
  bool InWall;

  void TryEnterWall() {
    if (InTransition)
      return;
    var validEntrance = Physics.Raycast(transform.position, transform.forward, out var hit, 1);
    if (!validEntrance) {
      Debug.LogWarning("not valid entrance");
    } else {
      InWall = true;
      StartCoroutine(EnterWall(hit.point, hit.normal));
    }
  }

  void TryExitWall() {
    if (InTransition)
      return;
    // TODO: Maybe should be volumetric like a box/sphere cast?
    var invalidExit = Physics.Raycast(transform.position, transform.forward, out var hit, 1);
    if (invalidExit) {
      Debug.LogWarning("not valid exit");
    } else {
      InWall = false;
      StartCoroutine(ExitWall(transform.position + transform.forward, -transform.forward));
    }
  }

  IEnumerator EnterWall(Vector3 position, Vector3 forward) {
    InTransition = true;
    for (var i = 0; i < WallTransitionDuration.Ticks; i++) {
      yield return new WaitForFixedUpdate();
    }
    transform.position = position;
    transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
    Model.SetActive(false);
    WallMover.enabled = true;
    WallCamera.Priority = 1;
    TopdownCamera.Priority = 0;
    InTransition = false;
  }

  IEnumerator ExitWall(Vector3 position, Vector3 forward) {
    InTransition = true;
    for (var i = 0; i < WallTransitionDuration.Ticks; i++) {
      yield return new WaitForFixedUpdate();
    }
    transform.position = position;
    transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
    Model.SetActive(true);
    WallMover.enabled = false;
    WallCamera.Priority = 0;
    TopdownCamera.Priority = 1;
    InTransition = false;
  }

  void Awake() {
    Inputs = new();
    Inputs.Enable();
    WallMover = GetComponent<WallMover>();
  }

  void OnDestroy() {
    Inputs.Disable();
    Inputs.Dispose();
  }

  void FixedUpdate() {
    var wallMerge = Inputs.Player.North.WasPerformedThisFrame();
    if (wallMerge) {
      if (InWall)
        TryExitWall();
      else
        TryEnterWall();
    }
    var move = Inputs.Player.Move.ReadValue<Vector2>();
    if (InWall) {
      WallMover.Velocity = WallSpeed * move.x;
    } else {
      transform.position += Time.fixedDeltaTime * GroundSpeed * move.XZ();
      if (move.sqrMagnitude > 0)
        transform.rotation = Quaternion.LookRotation(move.XZ(), Vector3.up);
    }
  }

  void OnDrawGizmos() {
    if (InWall) {
      RenderWallExitInfo();
    } else {
      RenderWallEnterInfo();
    }
  }

  static bool CapsuleColliderCast(
  CapsuleCollider capsuleCollider,
  Vector3 position,
  Vector3 direction,
  float maxDistance,
  out RaycastHit hit) {
    Vector3 point1;
    Vector3 point2;
    switch (capsuleCollider.direction) {
      case 0: // X-axis
        point1 = capsuleCollider.transform.TransformPoint(capsuleCollider.center + Vector3.left * (capsuleCollider.height / 2 - capsuleCollider.radius));
        point2 = capsuleCollider.transform.TransformPoint(capsuleCollider.center + Vector3.right * (capsuleCollider.height / 2 - capsuleCollider.radius));
        break;
      case 1: // Y-axis
        point1 = capsuleCollider.transform.TransformPoint(capsuleCollider.center + Vector3.down * (capsuleCollider.height / 2 - capsuleCollider.radius));
        point2 = capsuleCollider.transform.TransformPoint(capsuleCollider.center + Vector3.up * (capsuleCollider.height / 2 - capsuleCollider.radius));
        break;
      case 2: // Z-axis
        point1 = capsuleCollider.transform.TransformPoint(capsuleCollider.center + Vector3.back * (capsuleCollider.height / 2 - capsuleCollider.radius));
        point2 = capsuleCollider.transform.TransformPoint(capsuleCollider.center + Vector3.forward * (capsuleCollider.height / 2 - capsuleCollider.radius));
        break;
      default:
        throw new System.NotImplementedException("Unknown capsule direction!");
    }

    float radius = capsuleCollider.radius;
    Debug.DrawLine(point1, point2);
    return Physics.CapsuleCast(point1, point2, radius, direction, out hit, maxDistance);
  }

  void RenderWallEnterInfo() {
    var distance = 1;
    var start = transform.position;
    var direction = transform.forward;
    var didHit = Physics.Raycast(start, direction, out var hit, distance);
    if (didHit) {
      var meshFilters = hit.collider.GetComponentsInChildren<MeshFilter>();
      var color = Color.white;
      color.a = .2f;
      foreach (var mf in meshFilters) {
        var mesh = Application.isPlaying ? mf.mesh : mf.sharedMesh;
        Gizmos.color = color;
        Gizmos.DrawWireMesh(mesh, submeshIndex: -1, mf.transform.position, mf.transform.rotation, mf.transform.lossyScale);
      }
    }
  }

  void RenderWallExitInfo() {
    var distance = 1;
    var start = transform.position;
    var direction = transform.forward;
    var end = start + distance * direction;
    var didHit = CapsuleColliderCast(CapsuleCollider, start, direction, distance, out var hit);
    var color = didHit ? Color.red : Color.white;
    color.a = .2f;
    Gizmos.color = color;
    Gizmos.DrawWireMesh(CapsuleMesh, submeshIndex: -1, end, Quaternion.identity, Vector3.one);
    Handles.Label(end + Vector3.up, $"{(hit.collider ? hit.collider.name : default)}");
  }
}