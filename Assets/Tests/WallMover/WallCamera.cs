using Cinemachine;
using UnityEngine;

public class WallCameraExtension : CinemachineExtension {
  public WallMover WallMover;
  public float DistanceFromTarget = 5f;
  public float ZoomSpeed = 5;
  public LayerMask LayerMask;

  float TargetDistance;

  protected override void OnEnable() {
    TargetDistance = DistanceFromTarget;
  }

  protected override void PostPipelineStageCallback(
  CinemachineVirtualCameraBase vcam,
  CinemachineCore.Stage stage,
  ref CameraState state,
  float deltaTime) {
    if (stage == CinemachineCore.Stage.Aim) {
      if (!WallMover || !WallMover.enabled)
        return;
      var weightedNormal = WallMover.WeightedNormal;
      if (weightedNormal.sqrMagnitude <= 0)
        return;
      var didHit = Physics.Raycast(vcam.LookAt.position, weightedNormal, out var hit, DistanceFromTarget, LayerMask);
      var distance = 0f;
      if (didHit) {
        distance = hit.distance;
      } else {
        distance = DistanceFromTarget;
      }
      TargetDistance = Mathf.MoveTowards(TargetDistance, distance, Time.deltaTime * ZoomSpeed);
      state.RawPosition = vcam.LookAt.position + weightedNormal * TargetDistance;
      state.RawOrientation = Quaternion.LookRotation(-weightedNormal, Vector3.up);
    }
  }
}