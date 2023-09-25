using UnityEngine;

public class HitEffects : MonoBehaviour {
  [SerializeField] Combatant Combatant;
  [SerializeField] AudioSource SoundSource;
  [SerializeField] AudioClip SwordBlockSFX;
  [SerializeField] AudioClip DamageSFX;
  [SerializeField] GameObject BlockVFX;
  [SerializeField] GameObject DamageVFX;
  [SerializeField] float BlockCameraShakeIntensity = 5;
  [SerializeField] float DamageCameraShakeIntensity = 10;

  void Start() {
    Combatant.OnHit += OnHit;
  }

  void OnDestroy() {
    Combatant.OnHit -= OnHit;
  }

  void OnHit(HitEvent hitEvent) {
    if (hitEvent.NoDamage) {
      // Hack: would need a more robust system for hits of different types.
      if (SwordBlockSFX && hitEvent.HitConfig.HitType == HitConfig.Types.Sword)
        SoundSource.PlayOneShot(SwordBlockSFX);
      if (BlockVFX)
        Destroy(Instantiate(BlockVFX, hitEvent.Victim.transform.position + .5f * Vector3.up, hitEvent.Victim.transform.rotation), 2);
      CameraShaker.Instance.Shake(BlockCameraShakeIntensity);
    } else {
      if (DamageSFX)
        SoundSource.PlayOneShot(DamageSFX);
      if (DamageVFX)
          Destroy(Instantiate(DamageVFX, hitEvent.Victim.transform.position + .5f * Vector3.up, hitEvent.Victim.transform.rotation), 2);
      CameraShaker.Instance.Shake(DamageCameraShakeIntensity);
    }
  }
}
