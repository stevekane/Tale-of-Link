using UnityEngine;

public class LookAtCamera : MonoBehaviour {
  Camera Camera;

  [RuntimeInitializeOnLoadMethod]
  public static void Boot() {
    Debug.LogWarning("LookAtCamera uses Camera.main which is slow. Could store ref somewhere.");
  }

  void Start() {
    Camera = Camera.main;
  }

  void LateUpdate() {
    transform.LookAt(transform.position + Camera.transform.forward);
  }
}