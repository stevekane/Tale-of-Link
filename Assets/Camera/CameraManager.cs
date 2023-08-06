using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraManager : LevelManager<CameraManager> {
  [SerializeField] CinemachineVirtualCamera WorldSpaceCamera;
  [SerializeField] CinemachineVirtualCamera WallSpaceCamera;
  [SerializeField] Volume Volume;

  public Camera Camera;
  public CanvasGroup ScreenFadeOverlay;

  float FadeSpeed = 100;
  float TargetSaturation = 0;

  public void FadeOut(float Speed) {
    FadeSpeed = Speed * 100;
    TargetSaturation = -100;
  }
  public void FadeIn(float Speed) {
    FadeSpeed = Speed * 100;
    TargetSaturation = 0;
  }

  void Start() {
    PlayerManager.Instance.OnPlayerSpawn += OnPlayerSpawn;
  }

  void OnDestroy() {
    PlayerManager.Instance.OnPlayerSpawn -= OnPlayerSpawn;
  }

  void Update() {
    if (Volume.profile.TryGet(out ColorAdjustments colorAdjustments)) {
      colorAdjustments.saturation.value = Mathf.MoveTowards(colorAdjustments.saturation.value, TargetSaturation, Time.deltaTime * FadeSpeed);
    }
  }

  void OnPlayerSpawn(Player player) {
    player.GetComponent<WallSpaceController>().OnEnterWallSpace += OnEnterWallSpace;
    player.GetComponent<WorldSpaceController>().OnEnterWorldSpace += OnEnterWorldSpace;
    WorldSpaceCamera.Follow = player.transform;
    WallSpaceCamera.LookAt = player.transform;
    WallSpaceCamera.GetComponent<WallCameraExtension>().WallMover = player.GetComponent<WallSpaceController>();
  }

  void OnPlayerDespawn(Player player) {
    player.GetComponent<WallSpaceController>().OnEnterWallSpace -= OnEnterWallSpace;
    player.GetComponent<WorldSpaceController>().OnEnterWorldSpace -= OnEnterWorldSpace;
    WorldSpaceCamera.Follow = null;
    WallSpaceCamera.LookAt = null;
    WallSpaceCamera.GetComponent<WallCameraExtension>().WallMover = null;
  }

  void OnEnterWallSpace() {
    WorldSpaceCamera.Priority = 0;
    WallSpaceCamera.Priority = 1;
  }

  void OnEnterWorldSpace() {
    WorldSpaceCamera.Priority = 1;
    WallSpaceCamera.Priority = 0;
  }
}