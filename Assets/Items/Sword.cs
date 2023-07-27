using System.Threading.Tasks;
using UnityEngine;

public class Sword : ClassicAbility, IItemAbility {
  public Vector3 AttachOffsetTODO;
  public GameObject Model;
  public Hitbox Hitbox;
  TaskScope Scope = new();

  public AbilityAction Action => Main;
  [field: SerializeField] public IItemAbility.Buttons DefaultButtonAssignment { get; set; }

  void Start() {
    if (AbilityManager) {
      // TODO: Attach to player's hand.
      transform.localPosition = AttachOffsetTODO;
    }
  }
  void OnDestroy() => Scope.Dispose();

  public override async Task MainAction(TaskScope scope) {
    try {
      IsRunning = true;
      Model.SetActive(true);
      Hitbox.EnableCollision = true;
      await scope.Seconds(.5f);
    } finally {
      IsRunning = false;
      Model.SetActive(false);
      Hitbox.EnableCollision = false;
    }
  }
}