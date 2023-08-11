using UnityEngine;

public class HurtEffects : MonoBehaviour {
  [SerializeField] Combatant Combatant;
  [SerializeField] AudioSource BlockSound;
  [SerializeField] AudioSource DamageSound;
  [SerializeField] GameObject VFX;
  [SerializeField] Animator Animator;

  void Start() {
    Combatant.OnHurt += OnHurt;
  }

  void OnDestroy() {
    Combatant.OnHurt -= OnHurt;
  }

  void OnHurt(HitEvent hitEvent) {
    if (hitEvent.Blocked && BlockSound)
      BlockSound.PlayOneShot(BlockSound.clip);
    if (!hitEvent.Blocked && DamageSound)
      DamageSound?.PlayOneShot(DamageSound.clip);
    if (VFX)
      Destroy(Instantiate(VFX, transform.position + .5f * Vector3.up, transform.rotation));
    if (Animator)
      if (hitEvent.Blocked)
        Animator.SetTrigger("Block");
      else
        Animator.SetTrigger("Flinch");
  }
}