using UnityEngine;
using UnityEngine.UI;

public class MagicMeter : MonoBehaviour {
  [SerializeField] Magic Magic;
  [SerializeField] Image Frame;
  [SerializeField] Image Background;
  [SerializeField] Image CurrentMeter;
  [SerializeField] Image RecentMeter;
  [SerializeField] Color DrainedColor;
  [SerializeField] float ColorChangeSpeed = 1;
  [SerializeField] float Drained;

  Color OriginalFrameColor;
  Color OriginalBackgroundColor;
  Color OriginalCurrentColor;
  Color OriginalRecentColor;
  Color DrainedFrameColor;
  Color DrainedBackgroundColor;
  Color DrainedCurrentColor;
  Color DrainedRecentColor;

  float Total;
  float Current;

  void Awake() {
    OriginalFrameColor = Frame.color;
    OriginalBackgroundColor = Background.color;
    OriginalCurrentColor = CurrentMeter.color;
    OriginalRecentColor = RecentMeter.color;
    DrainedFrameColor = DrainedColor;
    DrainedBackgroundColor = DrainedColor;
    DrainedCurrentColor = DrainedColor;
    DrainedRecentColor = DrainedColor;
    DrainedFrameColor.a = OriginalFrameColor.a;
    DrainedBackgroundColor.a = OriginalBackgroundColor.a;
    DrainedCurrentColor.a = OriginalCurrentColor.a;
    DrainedRecentColor.a = OriginalRecentColor.a;
    Magic.OnSetTotal += OnSetTotal;
    Magic.OnSet += OnSet;
    Magic.OnChangeCurrent += OnChangeCurrent;
    Magic.OnChangeRecent += OnChangeRecent;
  }

  void OnDestroy() {
    Magic.OnSetTotal -= OnSetTotal;
    Magic.OnSet -= OnSet;
    Magic.OnChangeCurrent -= OnChangeCurrent;
    Magic.OnChangeRecent -= OnChangeRecent;
  }

  void Update() {
    Drained = Mathf.MoveTowards(Drained, Magic.IsEmpty ? 1 : 0, ColorChangeSpeed * Time.deltaTime);
    Frame.color = Color.Lerp(OriginalFrameColor, DrainedFrameColor, Drained);
    Background.color = Color.Lerp(OriginalBackgroundColor, DrainedBackgroundColor, Drained);
    CurrentMeter.color = Color.Lerp(OriginalCurrentColor, DrainedCurrentColor, Drained);
    RecentMeter.color = Color.Lerp(OriginalRecentColor, DrainedRecentColor, Drained);
  }

  void OnSetTotal(float total) => Total = total;
  void OnSet(float current) => CurrentMeter.rectTransform.anchorMax = new(1, current/Total);
  void OnChangeCurrent(float current) => CurrentMeter.rectTransform.anchorMax = new(1, current/Total);
  void OnChangeRecent(float recent) => RecentMeter.rectTransform.anchorMax = new(1, recent/Total);
}