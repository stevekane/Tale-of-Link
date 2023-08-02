using System;
using UnityEngine;

[Serializable]
public class FloorGroup {
  public float MinRenderHeight;
  public GameObject[] Chunks;
}

public class FloorManager : LevelManager<FloorManager> {
  public FloorGroup[] FloorGroups;
  public Player Player;

  void Start() {
    base.Awake();
    PlayerManager.Instance.OnPlayerSpawn += SpawnPlayer;
  }

  void SpawnPlayer(Player player) {
    Player = player;
  }

  void LateUpdate() {
    if (!Player)
      return;
    var height = Player.transform.position.y;
    foreach (var group in FloorGroups) {
      var isActive = height >= group.MinRenderHeight;
      foreach (var chunk in group.Chunks) {
        chunk.SetActive(isActive);
      }
    }
  }

  void DespawnPlayer(Player player) {
    FloorGroups.ForEach(group => group.Chunks.ForEach(chunk => chunk.SetActive(false)));
  }
}