using System.Collections;
using UnityEngine;
using Cinemachine;

[DefaultExecutionOrder(-700)]
public class SimpleCharacter : MonoBehaviour {
  [SerializeField] Timeval WallTransitionDuration;
  [SerializeField] GameObject Model;
  [SerializeField] float GroundSpeed = 5;
  [SerializeField] float WallSpeed = 3;
  [SerializeField] CinemachineVirtualCamera TopdownCamera;
  [SerializeField] CinemachineVirtualCamera WallCamera;

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
}