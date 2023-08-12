using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CameraConfine : MonoBehaviour {
  void OnTriggerEnter(Collider c) {
    CameraManager.Instance.ChangeConfine(GetComponent<Collider>());
  }

  void OnTriggerExit(Collider c) {
    CameraManager.Instance.ChangeConfine(null);
  }
}