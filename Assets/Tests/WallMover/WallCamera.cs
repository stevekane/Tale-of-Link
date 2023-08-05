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
  public bool UseRotationalSpeed;

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
      if (weightedNormal.sqrMagnitude <= 0)
        return;
      var direction = -weightedNormal;
      if (UseRotationalSpeed) {
        var targetRotation = Quaternion.LookRotation(-weightedNormal, Vector3.up);
        var nextRotation = Quaternion.RotateTowards(CurrentRotation, targetRotation, Time.deltaTime * RotationSpeed);
        direction = nextRotation * Vector3.forward;
      }
      var didHit = Physics.Raycast(vcam.LookAt.position, -direction, out var hit, DistanceFromTarget, LayerMask);
      var distance = 0f;
      if (didHit && !hit.collider.GetComponent<CameraIgnore>()) {
        distance = Mathf.Max(hit.distance, MinDistanceFromTarget);
      } else {
        distance = DistanceFromTarget;
      }
      TargetDistance = Mathf.MoveTowards(TargetDistance, distance, Time.deltaTime * ZoomSpeed);
      state.RawPosition = vcam.LookAt.position - TargetDistance * direction;
      state.RawOrientation = Quaternion.LookRotation(direction, Vector3.up);
    }
    if (stage == CinemachineCore.Stage.Aim) {
      CurrentRotation = state.RawOrientation;
    }
  }
}