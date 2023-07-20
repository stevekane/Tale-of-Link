using System.Threading.Tasks;
using UnityEngine;

public class PlayerSword : MonoBehaviour {
  public Player Player { get; internal set; }
  public bool IsRunning { get; private set; }
  public bool CanRun => !IsRunning;

  public GameObject Sword;
  public Hitbox Hitbox;
  TaskScope Scope = new();

  void Awake() {
    GetComponent<InputHandler>().OnSword += () => MaybeStart(OnSword);
  }
  void OnDestroy() => Scope.Dispose();

  void MaybeStart(TaskFunc func) {
    if (CanRun)
      Scope.Start(func);
  }

  async Task OnSword(TaskScope scope) {
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