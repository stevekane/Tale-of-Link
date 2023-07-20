using UnityEngine;
using TMPro;

public class WorldSpaceMessage : MonoBehaviour {
  [SerializeField] float Smoothing = .1f;
  [SerializeField] TextMeshPro Text;

  public string Message {
    get => Text.text;
    set => Text.text = value;
  }

  void Update() {
    var dt = Time.deltaTime;
    Text.transform.localScale = Vector3.Lerp(Text.transform.localScale, Vector3.zero, 1 - Mathf.Pow(Smoothing, dt));
  }
}