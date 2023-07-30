using UnityEngine;

[DefaultExecutionOrder(100000)]
public class FixedTickCounter : SingletonBehavior<FixedTickCounter> {
  void FixedUpdate() {
    Timeval.TickCount++;
  }
}