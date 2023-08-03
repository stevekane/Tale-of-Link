using UnityEngine;
using TMPro;

public enum DisplayState {
  Hidden,
  In,
  Hold,
  Out
}

public class FloorNotification : MonoBehaviour {
  [SerializeField] CanvasGroup CanvasGroup;
  [SerializeField] TextMeshProUGUI FloorText;
  [SerializeField] float HoldDuration = 3;
  [SerializeField] float FadeDuration = .5f;
  [SerializeField] DisplayState State;

  float Remaining;

  void Awake() {
    FloorManager.Instance.OnFloorChange += OnFloorChange;
  }

  void OnDestroy() {
    FloorManager.Instance.OnFloorChange -= OnFloorChange;
  }

  void Update() {
    Remaining = Mathf.Max(0, Remaining-Time.deltaTime);
    CanvasGroup.alpha = State switch {
      DisplayState.In => 1-Mathf.InverseLerp(0, FadeDuration, Remaining),
      DisplayState.Out => Mathf.InverseLerp(0, FadeDuration, Remaining),
      DisplayState.Hold => 1,
      _ => 0
    };
    if (Remaining <= 0) {
      if (State == DisplayState.In) {
        Remaining = HoldDuration;
        State = DisplayState.Hold;
      } else if (State == DisplayState.Hold) {
        Remaining = FadeDuration;
        State = DisplayState.Out;
      } else if (State == DisplayState.Out) {
        Remaining = 0;
        State = DisplayState.Hidden;
      }
    }
  }

  void OnFloorChange(int floorIndex) {
    FloorText.text = $"{floorIndex+1}F";
    Remaining = FadeDuration;
    State = DisplayState.In;
  }
}