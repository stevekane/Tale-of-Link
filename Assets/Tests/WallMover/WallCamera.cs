using Cinemachine;
using UnityEngine;

[DefaultExecutionOrder(5)]
public class WallCameraExtension : CinemachineExtension {
  public WallSpaceController WallMover;
  public float MinDistanceFromTarget = 3f;
  public float DistanceFromTarget = 5f;
  public float ZoomSpeed = 5;
  public float RotationSpeed = 90;
  public LayerMask LayerMask;

  Quaternion CurrentRotation;

  float TargetDistance;

  protected override void OnEnable() {
    TargetDistance = DistanceFromTarget;
  }

  protected override void PostPipelineStageCallback(
  CinemachineVirtualCameraBase vcam,
  CinemachineCore.Stage stage,
  ref CameraState state,
  float deltaTime) {
    if (stage == CinemachineCore.Stage.Body) {
      if (!WallMover)
        return;
      var weightedNormal = WallMover.WeightedNormal;
      var targetRotation = Quaternion.LookRotation(-weightedNormal, Vector3.up);
      var nextRotation = Quaternion.RotateTowards(CurrentRotation, targetRotation, Time.deltaTime * RotationSpeed);
      var direction = nextRotation * Vector3.forward;
      if (weightedNormal.sqrMagnitude <= 0)
        return;
      var didHit = Physics.Raycast(vcam.LookAt.position, -direction, out var hit, DistanceFromTarget, LayerMask);
      var distance = 0f;
      if (didHit && !hit.collider.GetComponent<CameraIgnore>()) {
        distance = Mathf.Max(hit.distance, MinDistanceFromTarget);
      } else {
        distance = DistanceFromTarget;
      }
      var zoomDistance = Mathf.MoveTowards(TargetDistance, distance, Time.deltaTime * ZoomSpeed);
      state.RawPosition = vcam.LookAt.position -direction * zoomDistance;
    }
    if (stage == CinemachineCore.Stage.Aim) {
      CurrentRotation = state.RawOrientation;
    }
  }
}