using UnityEngine;

[ExecuteAlways]
public class CameraFade : MonoBehaviour {
  [SerializeField] Transform Target;
  [SerializeField] Camera CurrentFloor;
  [SerializeField] Camera NextFloor;
  [SerializeField] Material FinalMaterial;
  [SerializeField] float FloorHeight = 4;
  [SerializeField] int TargetFloor;
  [SerializeField] int CameraFloor;
  [SerializeField] float Blend;

  void LateUpdate() {
    transform.position = Target.position + 6 * Vector3.up;
    TargetFloor = Mathf.FloorToInt(Target.position.y / FloorHeight) + 1;
    CameraFloor = Mathf.FloorToInt(transform.position.y / FloorHeight) + 1;
    CurrentFloor.nearClipPlane = (transform.position.y-TargetFloor*FloorHeight) + .02f;
    CurrentFloor.farClipPlane = transform.position.y + .02f;
    NextFloor.nearClipPlane = .02f;
    NextFloor.farClipPlane = CurrentFloor.nearClipPlane;
    var floorBlendMin = FloorHeight-1;
    var floorBlendMax = FloorHeight;
    var floorPosition = Target.position.y % FloorHeight;
    if (floorPosition <= floorBlendMax && floorPosition >= floorBlendMin) {
      Blend = Mathf.InverseLerp(floorBlendMin, floorBlendMax, floorPosition);
    } else if (CameraFloor == TargetFloor) {
      Blend = 0;
    } else {
      Blend = 0;
    }
    FinalMaterial.SetFloat("_Blend", Blend);
  }

  void OnDrawGizmos() {
    Gizmos.color = Color.blue;
    Gizmos.matrix = CurrentFloor.transform.localToWorldMatrix;
    Gizmos.DrawFrustum(Vector3.zero, CurrentFloor.fieldOfView, CurrentFloor.farClipPlane, CurrentFloor.nearClipPlane, CurrentFloor.aspect);
    var overlayColor = Color.red;
    Gizmos.color = overlayColor;
    Gizmos.matrix = NextFloor.transform.localToWorldMatrix;
    Gizmos.DrawFrustum(Vector3.zero, NextFloor.fieldOfView, NextFloor.farClipPlane, NextFloor.nearClipPlane, NextFloor.aspect);
  }
}