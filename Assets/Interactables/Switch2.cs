using UnityEngine;

public class Switch2 : MonoBehaviour {
  public Renderer Renderer;
  public Material[] Materials;
  public int State = 0;

  void Awake() {
    SetSwitchState(State, false);
  }

  public void SetSwitchState(int state, bool animate) {
    State = state;
    Renderer.material = Materials[state];
  }
}
