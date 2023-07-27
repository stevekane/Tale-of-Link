using UnityEngine;
using TMPro;

public class InteractionPrompt : MonoBehaviour {
  [SerializeField] InputManager InputManager;
  [SerializeField] CanvasGroup CanvasGroup;
  [SerializeField] TextMeshProUGUI InteractionMessage;
  [SerializeField] float FadeSpeed = 3;

  bool Displayed;

  void Awake() {
    InputManager.OnInteractChange += OnInteractChange;
  }

  void OnDestroy() {
    InputManager.OnInteractChange -= OnInteractChange;
  }

  void OnInteractChange(string message) {
    Displayed = message != "";
    InteractionMessage.text = message;
  }

  void LateUpdate() {
    CanvasGroup.alpha = Mathf.MoveTowards(CanvasGroup.alpha, Displayed ? 1 : 0, Time.deltaTime * FadeSpeed);
  }
}