using UnityEngine;

public class HurtEffects : MonoBehaviour {
  [SerializeField] Combatant Combatant;
  [SerializeField] AudioSource SoundSource;
  [SerializeField] AudioClip BlockSFX;
  [SerializeField] AudioClip DamageSFX;
  [SerializeField] GameObject BlockVFX;
  [SerializeField] GameObject DamageVFX;
  [SerializeField] Animator Animator;

  // Hack to allow Breakable SFX to outlive their owning object.
  AudioSource AudioSource => SoundSource ? SoundSource : AudioManager.Instance.SoundSource;

  void Start() {
    Combatant.OnHurt += OnHurt;
  }

  void OnDestroy() {
    Combatant.OnHurt -= OnHurt;
  }

  void OnHurt(HitEvent hitEvent) {
    if (hitEvent.NoDamage) {
      if (BlockSFX)
        AudioSource.PlayOneShot(BlockSFX);
      if (DamageVFX)
        Destroy(Instantiate(BlockVFX, transform.position + .5f * Vector3.up, transform.rotation), 2);
      if (Animator)
        Animator.SetTrigger("Block");
    } else {
      if (DamageSFX)
        AudioSource.PlayOneShot(DamageSFX);
      if (BlockVFX)
        Destroy(Instantiate(DamageVFX, transform.position + .5f * Vector3.up, transform.rotation), 2);
      if (Animator)
        Animator.SetTrigger("Flinch");
    }
  }
}