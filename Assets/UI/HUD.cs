using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class HUD : MonoBehaviour {
  void Start() {
    GetComponent<Canvas>().worldCamera = CameraManager.Instance.Camera;
  }
}