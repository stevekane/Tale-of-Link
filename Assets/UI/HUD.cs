using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class HUD : MonoBehaviour {
  [SerializeField] Killable Killable;
  [SerializeField] CanvasGroup HUDCanvasGroup;
  [SerializeField] CanvasGroup WastedCanvasGroup;
  [SerializeField] Button RestartButton;
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

  public void ShowGameOver() {
    RestartButton.gameObject.SetActive(true);
    EventSystem.current.SetSelectedGameObject(RestartButton.gameObject);
    StartCoroutine(FadeWasted());
  }

  const float AlphaSpeed = .01f;
  IEnumerator FadeWasted() {
    while (WastedCanvasGroup.alpha < 1f) {
      WastedCanvasGroup.alpha += AlphaSpeed;
      yield return new WaitForFixedUpdate();
    }
  }

  void Start() {
    GetComponent<Canvas>().worldCamera = CameraManager.Instance.Camera;
    Killable.OnDying += OnDying;
    Killable.OnSpawning += OnSpawning;
    WastedCanvasGroup.alpha = 0;
    HideCollectionInfo();
    RestartButton.onClick.AddListener(OnRestart);
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

  void OnRestart() {
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }
}