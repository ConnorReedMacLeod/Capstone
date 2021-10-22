using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewStartMatch : MonoBehaviour {
  
    public void OnMouseDown() {
        //Let the skill engine know that it can start processing its stack of effects (i.e., start executing the match)
        ContSkillEngine.Get().StartAutoProcessingStacks();

        //disable ourselves since we're not needed anymore (and since we don't want to allow double-clicking)
        this.gameObject.SetActive(false);
    }
}
