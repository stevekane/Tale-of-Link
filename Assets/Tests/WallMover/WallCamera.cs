using Cinemachine;
using UnityEngine;

public class WallCameraExtension : CinemachineExtension {
    public WallMover WallMover;
    public float DistanceFromTarget = 5f;

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
        state.RawPosition = vcam.Follow.position - weightedNormal * DistanceFromTarget;
        state.RawOrientation = Quaternion.LookRotation(weightedNormal, Vector3.up);
      }
  }
}