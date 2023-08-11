using UnityEngine;

public class HitEffects : MonoBehaviour {
  [SerializeField] Combatant Combatant;
  [SerializeField] AudioSource BlockSound;
  [SerializeField] AudioSource DamageSound;
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
      if (BlockSound)
        BlockSound?.PlayOneShot(BlockSound.clip);
      if (DamageVFX)
        Destroy(Instantiate(BlockVFX, hitEvent.Victim.transform.position + .5f * Vector3.up, hitEvent.Victim.transform.rotation), 2);
      CameraShaker.Instance.Shake(BlockCameraShakeIntensity);
    } else {
      if (DamageSound)
        DamageSound.PlayOneShot(DamageSound.clip);
      if (BlockVFX)
        Destroy(Instantiate(DamageVFX, hitEvent.Victim.transform.position + .5f * Vector3.up, hitEvent.Victim.transform.rotation), 2);
      CameraShaker.Instance.Shake(DamageCameraShakeIntensity);
    }
  }
}
