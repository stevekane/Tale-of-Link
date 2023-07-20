using UnityEngine;

public class Player : MonoBehaviour {
  [HideInInspector] public PlayerSword Sword;
  [HideInInspector] public PlayerMove Move;

  private void Awake() {
    this.InitComponent(out Sword); Sword.Player = this;
    this.InitComponent(out Move); Move.Player = this;
  }
}