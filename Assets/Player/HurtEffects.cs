using UnityEngine;

public class HurtEffects : MonoBehaviour {
  [SerializeField] Combatant Combatant;
  [SerializeField] AudioSource BlockSound;
  [SerializeField] AudioSource DamageSound;
  [SerializeField] GameObject VFX;
  [SerializeField] Animator Animator;
  [SerializeField] string BlockTriggerName = "Block";
  [SerializeField] string FlinchTriggerName = "Flinch";

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
      Destroy(Instantiate(VFX, transform.position, transform.rotation));
    if (Animator)
      if (hitEvent.Blocked)
        Animator.SetTrigger(BlockTriggerName);
      else
        Animator.SetTrigger(FlinchTriggerName);
  }
}