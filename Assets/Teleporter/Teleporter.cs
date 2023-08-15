using System.Threading.Tasks;
using UnityEngine;

public class Teleporter : ClassicAbility {
  [SerializeField] WorldSpaceController Controller;
  [SerializeField] Timeval FadeOutDuration = Timeval.FromSeconds(1);
  [SerializeField] Timeval FadeInDuration = Timeval.FromSeconds(1);
  [SerializeField] float Speed = 1;
  [SerializeField] float TurnSpeed = 360;

  TeleporterPad Source;

  void OnTriggerEnter(Collider c) {
    if (c.TryGetComponent(out TeleporterPad pad) && pad.Exit && AbilityManager.CanRun(Main)) {
      if (!Source || pad != Source.Exit) {
        Source = pad;
        AbilityManager.Run(Main);
      }
    }
  }

  void OnTriggerExit(Collider c) {
    if (c.TryGetComponent(out TeleporterPad pad) && Source && pad == Source.Exit) {
      Source = null;
    }
  }

  public override async Task MainAction(TaskScope scope) {
    try {
      Controller.DirectMove = true;
      for (var i = 0; i <= FadeOutDuration.Ticks; i++) {
        Controller.Position = Vector3.MoveTowards(Controller.Position, Source.transform.position, Time.fixedDeltaTime * Speed);
        Controller.Forward = Quaternion.RotateTowards(Controller.transform.rotation, Source.transform.rotation, Time.fixedDeltaTime * TurnSpeed) * Vector3.forward;
        CameraManager.Instance.ScreenFadeOverlay.alpha = (float)i/FadeOutDuration.Ticks;
        await scope.Tick();
      }
      Controller.Position = Source.Exit.transform.position;
      Controller.Forward = Source.Exit.transform.forward;
      for (var i = 0; i <= FadeInDuration.Ticks; i++) {
        CameraManager.Instance.ScreenFadeOverlay.alpha = 1f-(float)i/FadeInDuration.Ticks;
        await scope.Tick();
      }
    } catch (System.Exception e) {
      throw e;
    } finally {
      Controller.DirectMove = false;
    }
  }
}