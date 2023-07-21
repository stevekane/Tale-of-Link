using System.Threading.Tasks;
using UnityEngine;

public class PlayerHammer : MonoBehaviour {
  public Player Player { get; internal set; }
  public bool IsRunning { get; private set; }
  public bool CanRun => !IsRunning && !Player.Sword.IsRunning;

  public GameObject Hammer;
  public Hitbox Hitbox;
  TaskScope Scope = new();

  void Awake() {
    GetComponent<InputHandler>().OnHammer += () => TryStart(Run);
  }
  void OnDestroy() => Scope.Dispose();

  void TryStart(TaskFunc func) {
    if (CanRun)
      Scope.Start(func);
  }

  async Task Run(TaskScope scope) {
    try {
      IsRunning = true;
      Hammer.SetActive(true);
      Hitbox.EnableCollision = true;
      await scope.Seconds(.5f);
    } finally {
      IsRunning = false;
      Hammer.SetActive(false);
      Hitbox.EnableCollision = false;
    }
  }
}