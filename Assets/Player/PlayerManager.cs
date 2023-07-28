using UnityEngine.Events;

public class PlayerManager : LevelManager<PlayerManager> {
  public Player Player { get; private set; }
  public UnityAction<Player> OnPlayerSpawn;
  public UnityAction<Player> OnPlayerDespawn;

  public void SpawnPlayer(Player player) {
    Player = player;
    OnPlayerSpawn?.Invoke(Player);
  }

  public void DespawnPlayer(Player player) {
    OnPlayerDespawn?.Invoke(Player);
    Player = player;
  }
}