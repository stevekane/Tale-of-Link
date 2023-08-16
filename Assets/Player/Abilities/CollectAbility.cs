using System;
using System.Threading.Tasks;
using UnityEngine;

/*
TODO: VERY IMPORTANT:

Collecting multiple items this way simultaneously just won't work currently.
The collect ability somehow needs to have a queue of items that it must process.
If items are added to it while the ability is running, it should loop through those
items until the whole queue has been drained.
*/
public class CollectAbility : ClassicAbility {
  [SerializeField] ParticleSystem DisplayParticles;
  [SerializeField] float ForwardOffset = 1;
  [SerializeField] float UpwardOffset = 3;
  [SerializeField] float RotationSpeed = 180;
  [SerializeField] Timeval DisplayDuration = Timeval.FromSeconds(2.5f);

  public string DisplayText;
  public GameObject DisplayObject;
  public Action OnCollect;
  public override async Task MainAction(TaskScope scope) {
    var displayParticles = (ParticleSystem)default;
    var controller = AbilityManager.GetComponent<WorldSpaceController>();
    var equipmentVisibility = AbilityManager.GetComponent<EquipmentVisibility>();
    var animator = AbilityManager.GetComponent<Animator>();
    var hud = AbilityManager.GetComponentInChildren<HUD>();
    try {
      TimeManager.Instance.Frozen = true;
      TimeManager.Instance.IgnoreFreeze.Add(LocalTime);
      DisplayObject.SetActive(false);
      var rotationSteps = 0;
      while (controller.Forward != -Vector3.forward || rotationSteps++ > 60) {
        controller.Forward = Vector3.RotateTowards(controller.Forward, -Vector3.forward, Time.fixedDeltaTime * RotationSpeed * Mathf.Deg2Rad, 1);
        await scope.Tick();
      }
      hud.Hide();
      hud.DisplayCollectionInfo(DisplayText);
      CameraManager.Instance.ZoomIn();
      equipmentVisibility.DisplayNothing();
      animator.SetBool("Collecting", true);
      var displayPosition = AbilityManager.transform.position + UpwardOffset*Vector3.up + ForwardOffset*Vector3.forward;
      var displayRotation = CameraManager.Instance.Camera.transform.rotation;
      var displayParticlesPosition = displayPosition - .5f * Vector3.up;
      displayParticles = Instantiate(DisplayParticles, displayParticlesPosition, displayRotation);
      DisplayObject.SetActive(true);
      DisplayObject.transform.SetPositionAndRotation(displayPosition, displayRotation);
      await scope.Ticks(DisplayDuration.Ticks);
    } catch (Exception e) {
      throw e;
    } finally {
      TimeManager.Instance.Frozen = false;
      TimeManager.Instance.IgnoreFreeze.Remove(LocalTime);
      hud.Show();
      hud.HideCollectionInfo();
      Destroy(displayParticles.gameObject);
      CameraManager.Instance.ZoomOut();
      equipmentVisibility.DisplayBaseObjects();
      animator.SetBool("Collecting", false);
      OnCollect?.Invoke();
    }
  }
}