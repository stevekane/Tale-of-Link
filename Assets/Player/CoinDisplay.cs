using UnityEngine;
using TMPro;

public enum ScrollState {
  Hidden,
  SlideIn,
  Visible,
  SlideOut
}

public class CoinDisplay : MonoBehaviour {
  [SerializeField] RectTransform RectTransform;
  [SerializeField] Coins Coins;
  [SerializeField] TextMeshProUGUI[] Digits;
  [SerializeField] float BaseChangeSpeed = 10;
  [SerializeField] float ScrollSpeed = 1;
  [SerializeField] float VisibleDuration = 1;

  float Current;
  float TargetCurrent;
  float Speed;
  public float VisibleRemaining;
  public float Visibility;
  public ScrollState ScrollState;

  void Awake() {
    Coins.OnSetCurrent += SetCurrent;
    Coins.OnChangeCurrent += ChangeCurent;
    var height = RectTransform.rect.height;
    var position = RectTransform.anchoredPosition;
    position.y = height;
    RectTransform.anchoredPosition = position;
  }

  void OnDestroy() {
    Coins.OnSetCurrent -= SetCurrent;
    Coins.OnChangeCurrent -= ChangeCurent;
  }

  void Update() {
    switch (ScrollState) {
      case ScrollState.SlideIn: {
        var height = RectTransform.rect.height;
        var position = RectTransform.anchoredPosition;
        Visibility = Mathf.MoveTowards(Visibility, 1, ScrollSpeed * Time.deltaTime);
        position.y = (1f-Visibility) * height;
        RectTransform.anchoredPosition = position;
        if (Visibility == 1) {
          ScrollState = ScrollState.Visible;
          VisibleRemaining = VisibleDuration;
        }
      }
      break;

      case ScrollState.SlideOut: {
        var height = RectTransform.rect.height;
        var position = RectTransform.anchoredPosition;
        Visibility = Mathf.MoveTowards(Visibility, 0, ScrollSpeed * Time.deltaTime);
        position.y = (1f-Visibility) * height;
        RectTransform.anchoredPosition = position;
        if (Visibility == 0) {
          ScrollState = ScrollState.Hidden;
          VisibleRemaining = 0;
        }
      }
      break;

      case ScrollState.Visible:
        Current = Mathf.MoveTowards(Current, TargetCurrent, Speed * Time.deltaTime);
        VisibleRemaining = Mathf.MoveTowards(VisibleRemaining, Current == TargetCurrent ? 0 : VisibleDuration, Time.deltaTime);
        if (VisibleRemaining <= 0) {
          ScrollState = ScrollState.SlideOut;
          VisibleRemaining = 0;
        }
      break;
    }

    var currentWhole = Mathf.FloorToInt(Current);
    for (var i = 0; i < Digits.Length; i++) {
      Digits[i].text = (currentWhole % 10).ToString();
      currentWhole /= 10;
    }
  }

  void ChangeCurent(int current) {
    var delta = current - TargetCurrent;
    Speed = BaseChangeSpeed * Mathf.Log10(Mathf.Abs(delta * delta * delta) + 2);
    TargetCurrent = current;
    ScrollState = ScrollState.SlideIn;
  }

  void SetCurrent(int current) {
    TargetCurrent = current;
    Current = current;
  }
}