using UnityEngine;

public class Player : MonoBehaviour {
  public Transform CameraFocusTarget;

  void Start() {
    PlayerManager.Instance.SpawnPlayer(this);
  }

  void OnDestroy() {
    PlayerManager.Instance.DespawnPlayer(this);
  }
}