using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100000)]
public class TimeManager : SingletonBehavior<TimeManager> {
  static public int FIXED_FPS = 60;
  static public int FPS = 60;

  public bool Frozen;

  public readonly List<LocalTime> LocalTimes = new(1024);
  public readonly List<LocalTime> IgnoreFreeze = new(32);

  public float TimeScale(LocalTime lt) => (Frozen && !IgnoreFreeze.Contains(lt)) ? 0 : 1;

  [RuntimeInitializeOnLoadMethod]
  static void SetupApplicationFrameRates() {
    Application.targetFrameRate = FPS;
    Time.fixedDeltaTime = 1f/FIXED_FPS;
  }

  protected override void AwakeSingleton() {
    Timeval.TickCount = 0;
  }

  void Start() {
    Timeval.TickCount = 1;
  }
}