using System.Threading.Tasks;
using UnityEngine;

// Fundamental ability concepts:
// IsRunning: abilities and systems want to know when they/other abilities are running
// CanRun: can this ability start given the state of the character/game?
// maybe CanBeCancelled: can this ability be cancelled by the character?
// maybe CanBeInterrupted: can this ability be interrupted by an event? alternative could just be each ability listens for the events that can interrupt it.
public class PlayerSword : MonoBehaviour {
  public Player Player { get; internal set; }
  public bool IsRunning { get; private set; }
  public bool CanRun => !IsRunning;  // TODO: AbilityManager handles it
  //public bool CanRun => !IsRunning && !Player.Hammer.IsRunning;

  public GameObject Sword;
  public Hitbox Hitbox;
  TaskScope Scope = new();

  void Awake() {
    GetComponent<InputHandler>().OnSword += () => TryStart(Run);
  }

  void OnDestroy() => Scope.Dispose();

  void TryStart(TaskFunc func) {
    if (CanRun)
      Scope.Start(func);
  }

  async Task Run(TaskScope scope) {
    try {
      IsRunning = true;
      Sword.SetActive(true);
      Hitbox.EnableCollision = true;
      await scope.Seconds(.5f);
    } finally {
      IsRunning = false;
      Sword.SetActive(false);
      Hitbox.EnableCollision = false;
    }
  }
}