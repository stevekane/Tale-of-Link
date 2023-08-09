using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class HUD : MonoBehaviour {
  [SerializeField] Killable Killable;
  [SerializeField] CanvasGroup HUDCanvasGroup;

  public void Show() {
    HUDCanvasGroup.alpha = 1;
  }

  public void Hide() {
    HUDCanvasGroup.alpha = 0;
  }

  void Start() {
    GetComponent<Canvas>().worldCamera = CameraManager.Instance.Camera;
    Killable.OnDying += OnDying;
    Killable.OnSpawning += OnSpawning;
  }

  void OnDestroy() {
    Killable.OnDying -= OnDying;
    Killable.OnAlive -= OnSpawning;
  }

  void OnSpawning() {
    HUDCanvasGroup.alpha = 1;
  }

  void OnDying() {
    HUDCanvasGroup.alpha = 0;
  }
}