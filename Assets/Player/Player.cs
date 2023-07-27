using UnityEngine;

public class Player : MonoBehaviour {
  void Start() {
    PlayerManager.Instance.SpawnPlayer(this);
  }

  void OnDestroy() {
    PlayerManager.Instance.DespawnPlayer(this);
  }
}