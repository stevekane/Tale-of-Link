using UnityEngine;

public abstract class LevelManager<T> : MonoBehaviour where T : MonoBehaviour {
  public static T Instance;

  protected virtual void Awake() {
    Instance = this as T;
  }
}