using System.Threading.Tasks;
using UnityEngine;

public class SwitchEncounter : TaskRunnerComponent {
  [SerializeField] PathController Platform;

  public void Run() => StartTask(Encounter);

  async Task Encounter(TaskScope scope) {
    var player = PlayerManager.Instance.Player;
    TimeManager.Instance.Frozen = true;
    TimeManager.Instance.IgnoreFreeze.Add(LocalTime);
    TimeManager.Instance.IgnoreFreeze.Add(player.GetComponent<LocalTime>());
    TimeManager.Instance.IgnoreFreeze.Add(Platform.GetComponent<LocalTime>());
    CameraManager.Instance.FocusOn(Platform.transform);
    await scope.Seconds(1f);
    Platform.Activate();
    await scope.Seconds(2f);
    TimeManager.Instance.Frozen = false;
    TimeManager.Instance.IgnoreFreeze.Clear();
    CameraManager.Instance.UnFocus();
  }
}