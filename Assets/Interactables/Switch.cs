using UnityEngine;

public class Switch : MonoBehaviour {
  public Renderer Renderer;
  public Material MaterialZero;
  public Material MaterialOne;
  int State = 0;

  public void SetSwitchState(int state, bool animate) {
    State = state;
    Renderer.material = State == 1 ? MaterialOne : MaterialZero;
  }
}
