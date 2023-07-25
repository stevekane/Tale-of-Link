using System.Collections;
using UnityEngine;
using Cinemachine;
using KinematicCharacterController;
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
  [SerializeField] float EnterDistance = 1;
  [SerializeField] float ExitDistance = 1;

  Inputs Inputs;
  WallMover WallMover;
  bool InTransition;
  bool InWall;

  void TryEnterWall() {
    if (InTransition)
      return;
    var validEntrance = Physics.Raycast(transform.position, transform.forward, out var hit, EnterDistance);
    if (!validEntrance || hit.collider.CompareTag("Blocker")) {
      Debug.LogWarning("not valid entrance");
    } else {
      InWall = true;
      GetComponent<Controller>().IgnoreCollision = true;
      GetComponent<Controller>().DirectMove = true;
      StartCoroutine(EnterWall(hit.point, hit.normal));
    }
  }

  void TryExitWall() {
    if (InTransition)
      return;
    var start = transform.position;
    var direction = transform.forward;
    var invalidExit = CapsuleCollider.CapsuleColliderCast(start, direction, ExitDistance, out var hit);
    if (invalidExit) {
      Debug.LogWarning("not valid exit");
    } else {
      InWall = false;
      GetComponent<Controller>().IgnoreCollision = false;
      GetComponent<Controller>().DirectMove = false;
      StartCoroutine(ExitWall(transform.position + transform.forward, -transform.forward));
    }
  }

  IEnumerator EnterWall(Vector3 position, Vector3 forward) {
    InTransition = true;
    for (var i = 0; i < WallTransitionDuration.Ticks; i++) {
      yield return new WaitForFixedUpdate();
    }
    transform.SetPositionAndRotation(position, Quaternion.LookRotation(forward, Vector3.up));
    GetComponent<Controller>().enabled = false;
    GetComponent<KinematicCharacterMotor>().enabled = false;
    // GetComponent<Controller>().Position = position;
    // GetComponent<Controller>().Forward = forward;
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
    // GetComponent<Controller>().Position = position;
    // GetComponent<Controller>().Forward = forward;
    transform.SetPositionAndRotation(position, Quaternion.LookRotation(forward, Vector3.up));
    GetComponent<Controller>().enabled = true;
    GetComponent<KinematicCharacterMotor>().enabled = true;
    GetComponent<KinematicCharacterMotor>().SetPosition(position);
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
      GetComponent<Controller>().Velocity = GroundSpeed * move.XZ();
      if (move.sqrMagnitude > 0)
        GetComponent<Controller>().Forward = move.XZ();
    }
    if (Inputs.Player.L1.WasPerformedThisFrame()) {
      GetComponent<Magic>().Consume(25);
    }
    if (Inputs.Player.L2.WasPerformedThisFrame()) {
      GetComponent<Magic>().Restore(101);
    }
    if (Inputs.Player.R2.WasPerformedThisFrame()) {
    }
  }

  void OnDrawGizmos() {
    if (InWall) {
      RenderWallExitInfo();
    } else {
      RenderWallEnterInfo();
    }
  }

  void RenderWallEnterInfo() {
    var distance = EnterDistance;
    var start = transform.position;
    var direction = transform.forward;
    var didHit = Physics.Raycast(start, direction, out var hit, distance);
    if (didHit && !hit.collider.CompareTag("Blocker")) {
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
    var distance = ExitDistance;
    var start = transform.position;
    var direction = transform.forward;
    var end = start + distance * direction;
    var didHit = CapsuleCollider.CapsuleColliderCast(start, direction, distance, out var hit);
    var color = didHit ? Color.red : Color.white;
    color.a = .2f;
    Gizmos.color = color;
    Gizmos.DrawWireMesh(CapsuleMesh, submeshIndex: -1, end, Quaternion.identity, Vector3.one);
    Handles.Label(end + Vector3.up, $"{(hit.collider ? hit.collider.name : default)}");
  }
}