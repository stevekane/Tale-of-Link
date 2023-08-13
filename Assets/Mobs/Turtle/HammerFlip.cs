using System.Threading.Tasks;
using UnityEngine;

public class HammerFlip : StunMob {
  public GameObject Model;
  public Hearts Hearts;
  public bool IsFlipped = false;
  public float Height = 1f;

  void Start() {
    Hearts.IsInvulnerable = true;
  }

  public override async Task MainAction(TaskScope scope) {
    try {
      IsFlipped = !IsFlipped;
      if (IsFlipped) {
        Model.transform.Rotate(0, 0, 180);
        Model.transform.position += new Vector3(0, Height, 0);
        Hearts.IsInvulnerable = false;
        await scope.Delay(StunDuration);
      }
      if (Hearts.Current > 0)  // dumb hack to only recover while alive
        Recover();
    } finally {
    }
  }

  void Recover() {
    IsFlipped = false;
    Model.transform.Rotate(0, 0, 180);
    Model.transform.position -= new Vector3(0, Height, 0);
    Hearts.IsInvulnerable = true;
  }
}