using KinematicCharacterController;
using System;
using UnityEngine;

public class AIWander : MonoBehaviour {
  public WorldSpaceController Controller;
  public AbilityManager AbilityManager;
  public WorldSpaceMove Move;
  public Timeval ChooseTargetCooldown;

  void Start() {
    Controller.OnCollision += OnCollision;
  }

  Vector3 TargetDir;
  int ChooseTicksRemaining = 0;
  void FixedUpdate() {
    if (--ChooseTicksRemaining < 0) {
      TargetDir = UnityEngine.Random.onUnitSphere.XZ().normalized;
      ChooseTicksRemaining = ChooseTargetCooldown.Ticks;
    }
    if (AbilityManager.CanRun(Move.Move)) {
      AbilityManager.Run(Move.Move, TargetDir);
    }
  }

  void OnCollision(HitStabilityReport obj) {
    ChooseTicksRemaining = 0;
  }
}