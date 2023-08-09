using UnityEngine;

[RequireComponent(typeof(Openable))]
public class Reward : MonoBehaviour {
  [SerializeField] GameObject Award;

  Openable Openable;

  void Awake() {
    this.InitComponent(out Openable);
  }

  public void Grant() {
    if (Award && Openable.Opener) {
      Instantiate(Award, Openable.Opener.transform.position, Quaternion.identity);
    }
  }
}