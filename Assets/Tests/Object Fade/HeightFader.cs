using UnityEngine;

[DefaultExecutionOrder(1)]
public class HeightFader : MonoBehaviour {
  Renderer[] Renderers;

  void Awake() {
    Renderers = GetComponentsInChildren<Renderer>();
  }

  void Start() {
    HeightFaderManager.Instance.HeightFaders.Add(this);
  }

  void OnDestroy() {
    HeightFaderManager.Instance.HeightFaders.Remove(this);
  }

  public void SetAlpha(float alpha) {
    Renderers.ForEach(r => r.material.SetFloat("_Alpha", alpha));
  }

  public void SetAlpha(float alpha, float speed) {
    Renderers.ForEach(r => r.material.SetFloat("_Alpha", Mathf.MoveTowards(r.material.GetFloat("_Alpha"), alpha, Time.deltaTime * speed)));
  }
}