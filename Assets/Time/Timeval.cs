using System;
using UnityEngine;

[Serializable]
public class Timeval {
  public static int FixedUpdatePerSecond = 60;
  public static int TickCount = 0;
  //public static EventSource TickEvent = new();

  [SerializeField] public float Millis = 1;

  public static Timeval FromSeconds(float seconds) {
    return new Timeval { Millis = seconds*1000f };
  }
  public static Timeval FromMillis(float millis) {
    return new Timeval { Millis = millis };
  }
  public static Timeval FromTicks(int frames) {
    return new Timeval { Millis = (float)frames * 1000f / FixedUpdatePerSecond };
  }

  public int Ticks {
    set { Millis = value * 1000f / FixedUpdatePerSecond; }
    get { return Mathf.RoundToInt(Millis * FixedUpdatePerSecond / 1000f); }
  }
  public float Seconds {
    get { return Millis * .001f; }
  }
}