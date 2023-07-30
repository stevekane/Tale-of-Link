using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class HeightFaderManager : LevelManager<HeightFaderManager> {
  [SerializeField] Transform Target;
  [SerializeField] float FadeDistance = 1;
  [SerializeField] float Speed = 1;

  public List<HeightFader> HeightFaders;

  void Start() {
    foreach (var fader in HeightFaders) {
      fader.SetAlpha(0);
    }
  }

  void LateUpdate() {
    foreach (var fader in HeightFaders) {
      fader.SetAlpha(Mathf.InverseLerp(FadeDistance, 0, fader.transform.position.y - Target.position.y), Speed);
    }
  }
}