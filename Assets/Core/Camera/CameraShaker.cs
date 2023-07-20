using Cinemachine;
using UnityEngine;

public class CameraConfig {
  [Tooltip("Rate of time dilation decay")]
  [Range(-10, 0)]
  public float TIME_DELATION_DECAY_EPSILON = -0.5f;

  [Tooltip("Rate of interpolation for camera lookahead")]
  [Range(-10, 0)]
  public float LOOK_AHEAD_EPSILON = -0.5f;

  [Tooltip("Rate of interpolation for camera shake decay")]
  [Range(-10, 0)]
  public float SHAKE_DECAY_EPSILON = -0.5f;

  [Tooltip("Max intensity of camera shake")]
  [Range(0, 100)]
  public float MAX_SHAKE_INTENSITY = 5;
}

public class CameraShaker : CinemachineExtension {
  [SerializeField] CameraConfig Config;
  CinemachineVirtualCamera TargetCamera;
  CinemachineBasicMultiChannelPerlin Noise;

  public static CameraShaker Instance;

  public void Shake(float targetIntensity) {
    Noise.m_AmplitudeGain = Mathf.Min(Noise.m_AmplitudeGain+targetIntensity, Config.MAX_SHAKE_INTENSITY);
  }

  protected override void Awake() {
    base.Awake();
    Instance = this;
  }

  protected override void ConnectToVcam(bool connect) {
    base.ConnectToVcam(connect);
    TargetCamera = VirtualCamera as CinemachineVirtualCamera;
    Noise = TargetCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
  }

  protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float dt) {
    Noise.m_AmplitudeGain = Mathf.Lerp(0, Noise.m_AmplitudeGain, Mathf.Exp(dt*Config.SHAKE_DECAY_EPSILON));
  }
}