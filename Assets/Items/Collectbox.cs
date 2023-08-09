using System;
using UnityEngine;

/*
This class really just exists to invoke the Collect ability.

It's possible this thing isn't necessary?
I'm unsure but it exists here as an intermediary for the moment
to avoid changing the existing code too much and because I am in doubt
about the optimal design.
*/
public class Collectbox : MonoBehaviour {
  [SerializeField] AbilityManager AbilityManager;
  [SerializeField] CollectAbility CollectAbility;

  public GameObject Owner;

  public void Collect(GameObject displayObject, string displayText, Action onCollect, bool playAnimation = false) {
    if (playAnimation && AbilityManager.CanRun(CollectAbility.Main)) {
      CollectAbility.DisplayText = displayText;
      CollectAbility.DisplayObject = displayObject;
      CollectAbility.OnCollect = onCollect;
      AbilityManager.Run(CollectAbility.Main);
    } else {
      onCollect?.Invoke();
    }
  }
}