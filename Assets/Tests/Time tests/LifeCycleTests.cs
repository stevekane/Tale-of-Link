using UnityEngine;

[DefaultExecutionOrder(2)]
public class LifeCycleTests : MonoBehaviour {
  public static void Print(string label) {
    Debug.Log($"{label}   FRAME:{Time.frameCount} | TICK:{Timeval.TickCount}");
  }

  bool PrintStart;
  bool PrintUpdate;
  bool PrintFixedUpdate;
  bool PrintLateUpdate;
  void OnEnable() {
    Print("Enable");
    PrintStart = PrintUpdate = PrintFixedUpdate = PrintLateUpdate = true;
  }

  void OnDisable() {
    PrintStart = PrintUpdate = PrintFixedUpdate = PrintLateUpdate = false;
  }

  void Start() {
    if (PrintStart) {
      Print("Start");
      PrintStart = false;
    }
  }

  void Update() {
    if (PrintUpdate) {
      Print("Update");
      PrintUpdate = false;
    }
  }

  void FixedUpdate() {
    if (PrintFixedUpdate) {
      Print("FixedUpdate");
      PrintFixedUpdate = false;
    }
  }

  void LateUpdate() {
    if (PrintLateUpdate) {
      Print("LateUpdate");
      PrintLateUpdate = false;
    }
  }
}