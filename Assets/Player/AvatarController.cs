using UnityEngine;

public class AvatarController : MonoBehaviour {
  [SerializeField] WorldSpaceController WorldSpaceController;
  [SerializeField] WallSpaceController WallSpaceController;
  [SerializeField] GameObject WorldSpaceAvatar;
  [SerializeField] GameObject WallSpaceAvatar;
  [SerializeField] bool StartInWorldSpace;

  void Start() {
    WorldSpaceController.OnEnterWorldSpace += OnEnterWorldSpace;
    WallSpaceController.OnEnterWallSpace += OnEnterWallSpace;
    WorldSpaceController.enabled = StartInWorldSpace;
    WallSpaceController.enabled = !StartInWorldSpace;
  }

  void OnEnterWorldSpace() {
    WorldSpaceAvatar.SetActive(true);
    WallSpaceAvatar.SetActive(false);
  }

  void OnEnterWallSpace() {
    WorldSpaceAvatar.SetActive(false);
    WallSpaceAvatar.SetActive(true);
  }
}