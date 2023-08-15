using UnityEngine;

public class LocalTime : MonoBehaviour {
  public float TimeScale => TimeManager.Instance.TimeScale(this);
  public float FixedDeltaTime => TimeScale * Time.fixedDeltaTime;
  public float DeltaTime => TimeScale * Time.deltaTime;

  void Start() {
    TimeManager.Instance.LocalTimes.Add(this);
  }

  void OnDestroy() {
    TimeManager.Instance.LocalTimes.Remove(this);
  }
}