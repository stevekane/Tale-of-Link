using UnityEngine;

public class Switch : MonoBehaviour {
  public Hurtbox Hurtbox;
  public Renderer Renderer;
  public Material MaterialZero;
  public Material MaterialOne;
  public bool State = false;

  private void Awake() {
    GetComponent<Combatant>().OnHurt += OnHurt;
    Renderer.material = State ? MaterialOne : MaterialZero;
  }

  void OnHurt(HitEvent _) {
    State = !State;
    Renderer.material = State ? MaterialOne : MaterialZero;
  }
}
