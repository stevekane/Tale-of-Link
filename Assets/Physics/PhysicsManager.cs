using UnityEngine;
using KinematicCharacterController;

public class PhysicsManager : SingletonBehavior<PhysicsManager> {
  public bool Interpolate = true;
  void Start() {
    KinematicCharacterSystem.Settings = ScriptableObject.CreateInstance<KCCSettings>();
    KinematicCharacterSystem.Settings.Interpolate = Interpolate;
  }
}