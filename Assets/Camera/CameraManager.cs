using UnityEngine;
using Cinemachine;

public class CameraManager : LevelManager<CameraManager> {
  public Camera Camera;
  public CinemachineVirtualCamera WorldSpaceCamera;
  public CinemachineVirtualCamera WallSpaceCamera;
  public CanvasGroup ScreenFadeOverlay;

  protected override void Awake() {
    base.Awake();
    PlayerManager.Instance.OnPlayerSpawn += OnPlayerSpawn;
  }

  void OnDestroy() {
    PlayerManager.Instance.OnPlayerSpawn -= OnPlayerSpawn;
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