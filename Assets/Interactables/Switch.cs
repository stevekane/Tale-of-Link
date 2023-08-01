using UnityEngine;

public class Switch : MonoBehaviour {
  public Renderer Renderer;
  public Material RedMaterial;
  public Material BlueMaterial;

  public void SetSwitchState(bool redActive, bool animate) {
    Renderer.material = redActive ? RedMaterial : BlueMaterial;
  }
}