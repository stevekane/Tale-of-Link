using UnityEngine;

public class StrobeGroup : MonoBehaviour {
  [SerializeField] float Period = 2;
  [SerializeField] float Delay = .25f;
  [SerializeField] float Min = 0;
  [SerializeField] float Max = 2;
  [SerializeField] Color Color = Color.white;
  [SerializeField] Color EmissionColor = Color.white;

  MeshRenderer[] MeshRenderers;

  void Awake() {
    var switchBlocks = GetComponentsInChildren<SwitchBlock>();
    MeshRenderers = new MeshRenderer[switchBlocks.Length];
    for (var i = 0 ; i < switchBlocks.Length; i++) {
      MeshRenderers[i] = switchBlocks[i].GetComponentInChildren<MeshRenderer>();
    }
  }

  void Update() {
    var t = 2 * Mathf.PI * Time.time / Period;
    for (var i = 0; i < MeshRenderers.Length; i++) {
      var mr = MeshRenderers[i];
      var interpolant = Mathf.InverseLerp(-1, 1, Mathf.Sin(t + Delay * i));
      var intensity = Mathf.Lerp(Min, Max, interpolant);
      mr.material.color = interpolant * Color;
      mr.material.SetColor("_EmissionColor", intensity * EmissionColor);
    }
  }
}