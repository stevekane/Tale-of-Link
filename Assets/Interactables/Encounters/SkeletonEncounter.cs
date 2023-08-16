using System.Threading.Tasks;
using UnityEngine;

public class SkeletonEncounter : TaskRunnerComponent {
  void SetPlayer(Player player, bool enabled) {
    var abilityManager = player.GetComponent<AbilityManager>();
    abilityManager.Abilities.ForEach(abilityManager.Stop);
    abilityManager.SetTag(AbilityTag.CanAttack | AbilityTag.CanMove | AbilityTag.CanRotate | AbilityTag.CanUseItem, false);
  }

  async Task Encounter(TaskScope scope) {
    try {
      // Enable Link
      SetPlayer(PlayerManager.Instance.Player, enabled:false);
    } finally {
      // Disable Link
      SetPlayer(PlayerManager.Instance.Player, enabled:true);
    }
    // Close Door
    // Camera show skeletons
    // Skeletons play animation
    // Play encounter music
    // Wait for dead skeletons
    // Wait 2 seconds
    // Camera show portal
    // Camera show elevator
    // Portal spawn
    // Camera show elevator
    // Play unlock Jingle
    // Activate elevator
    // Open Door
    // Enable Link
  }
}