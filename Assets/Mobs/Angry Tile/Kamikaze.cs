using UnityEngine;

[RequireComponent(typeof(Killable))]
[RequireComponent(typeof(Combatant))]
public class Kamikaze : MonoBehaviour {
  void Kill(HitEvent _) => GetComponent<Killable>().Dead = true;
  void Start() => GetComponent<Combatant>().OnHit += Kill;
  void OnDestroy() => GetComponent<Combatant>().OnHit -= Kill;
}