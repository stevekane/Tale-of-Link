using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Encounter : MonoBehaviour {
  static bool IsDead(GameObject o) => o == null;

  public UnityEvent OnBegin;
  public UnityEvent OnEnd;
  public GameObject[] TrackedObjects;
  public bool Active {
    get => active;
    set {
      if (value)
        OnBegin?.Invoke();
      else
        OnEnd?.Invoke();
      active = value;
    }
  }

  bool active;

  void Start() {
    OnBegin.AddListener(() => Debug.Log("Begin"));
    OnEnd.AddListener(() => Debug.Log("End"));
  }

  void FixedUpdate() {
    if (Active && TrackedObjects.All(IsDead)) {
      Active = false;
    }
  }
}