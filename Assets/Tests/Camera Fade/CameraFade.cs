using UnityEngine;

[ExecuteAlways]
public class CameraFade : MonoBehaviour {
  [SerializeField] Camera Base;
  [SerializeField] Camera Overlay;
  [SerializeField] Material FinalMaterial;
  [SerializeField] float FloorHeight = 4;
  [Range(0,1)]
  [SerializeField] float Blend;

  // Overlay camera only renders the current floor
  // Base camera renders everything below the current floor
  void Update() {
    var floor = Mathf.FloorToInt(transform.position.y / FloorHeight);
    Overlay.nearClipPlane = .02f;
    Overlay.farClipPlane = transform.position.y % FloorHeight + .02f;
    Base.nearClipPlane = Overlay.farClipPlane;
    Base.farClipPlane = transform.position.y + .02f;
    Blend = Mathf.Clamp01(transform.position.y > FloorHeight ? (transform.position.y % FloorHeight) : 1);
    FinalMaterial.SetFloat("_Blend", Blend);
  }
}