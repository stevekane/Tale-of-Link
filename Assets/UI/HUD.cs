using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class HUD : MonoBehaviour {
  [SerializeField] Killable Killable;
  [SerializeField] CanvasGroup HUDCanvasGroup;
  [SerializeField] CanvasGroup CollectionInfoCanvasGroup;
  [SerializeField] CollectionInfo CollectionInfo;

  public void Show() {
    HUDCanvasGroup.alpha = 1;
  }

  public void Hide() {
    HUDCanvasGroup.alpha = 0;
  }

  public void DisplayCollectionInfo(string text) {
    CollectionInfo.SetInfo(text);
    CollectionInfoCanvasGroup.alpha = 1;
  }

  public void HideCollectionInfo() {
    CollectionInfoCanvasGroup.alpha = 0;
  }

  void Start() {
    GetComponent<Canvas>().worldCamera = CameraManager.Instance.Camera;
    Killable.OnDying += OnDying;
    Killable.OnSpawning += OnSpawning;
    HideCollectionInfo();
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