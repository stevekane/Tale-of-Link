using UnityEngine;
using UnityEngine.UI;

public class HeartContainer : MonoBehaviour {
  [SerializeField] RectTransform RectTransform;
  [SerializeField] Image FillImage;
  [SerializeField] float CyclePeriod;
  [SerializeField] AnimationCurve HeartSizeOverCyclePeriod;

  public bool Beating;
  public float TargetFill {
    get => FillImage.fillAmount;
    set => FillImage.fillAmount = value;
  }

  void LateUpdate() {
    var frequency = 1.0f / CyclePeriod;
    var t = Time.time * frequency;
    var value = Mathf.Abs(Mathf.Sin(2 * Mathf.PI * t));
    var size = HeartSizeOverCyclePeriod.Evaluate(value);
    RectTransform.localScale = Beating ? size * Vector3.one : Vector3.one;
  }
}