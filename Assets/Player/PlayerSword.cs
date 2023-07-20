using System.Threading.Tasks;
using UnityEngine;

public class PlayerSword : MonoBehaviour {
  public GameObject Sword;
  public Hitbox Hitbox;
  TaskScope Scope = new();

  void Awake() {
    GetComponent<InputHandler>().OnSword += () => Scope.Start(OnSword);
  }
  void OnDestroy() => Scope.Dispose();

  async Task OnSword(TaskScope scope) {
    try {
      Sword.SetActive(true);
      Hitbox.EnableCollision = true;
      await scope.Seconds(.5f);
    } finally {
      Sword.SetActive(false);
      Hitbox.EnableCollision = false;
    }
  }
}