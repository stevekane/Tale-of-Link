using UnityEngine;

public class CameraTiltModel : MonoBehaviour {
  [SerializeField] Quaternion CameraTilt;
  [SerializeField] AnimationCurve TiltNorthBias;
  //[SerializeField] Vector3 CameraOffset;

  void FixedUpdate() {
    var dot = Vector3.Dot(transform.parent.forward, CameraManager.Instance.Camera.transform.up);
    // -dot: 1=south, -1=north
    var tilt = Quaternion.Lerp(Quaternion.identity, CameraTilt, TiltNorthBias.Evaluate(-dot));
    transform.rotation = tilt * transform.parent.rotation;
    // TODO: this hack ensures the model doesn't clip into objects to the north. but it makes him hang over ledges to the south.
    //transform.localPosition = Quaternion.Inverse(transform.parent.rotation) * CameraOffset;
  }
}