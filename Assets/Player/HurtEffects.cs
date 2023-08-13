using UnityEngine;

public class HurtEffects : MonoBehaviour {
  [SerializeField] Combatant Combatant;
  [SerializeField] AudioSource BlockSound;
  [SerializeField] AudioSource DamageSound;
  [SerializeField] GameObject BlockVFX;
  [SerializeField] GameObject DamageVFX;
  [SerializeField] Animator Animator;

  void Start() {
    Combatant.OnHurt += OnHurt;
  }

  void OnDestroy() {
    Combatant.OnHurt -= OnHurt;
  }

  void OnHurt(HitEvent hitEvent) {
    if (hitEvent.NoDamage) {
      if (BlockSound)
        BlockSound?.PlayOneShot(BlockSound.clip);
      if (DamageVFX)
        Destroy(Instantiate(BlockVFX, transform.position + .5f * Vector3.up, transform.rotation), 2);
      if (Animator)
        Animator.SetTrigger("Block");
    } else {
      if (DamageSound)
        DamageSound.PlayOneShot(DamageSound.clip);
      if (BlockVFX)
        Destroy(Instantiate(DamageVFX, transform.position + .5f * Vector3.up, transform.rotation), 2);
      if (Animator)
        Animator.SetTrigger("Flinch");
    }
  }
}