using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputAbilitySelection : MonoBehaviour {

    public Player plyrOwner;


    public virtual void ResetTargets() {

    }

    public abstract void StartSelection();

    public abstract void GaveInvalidTarget();

    public void SetOwner(Player _plyrOwner) {
        plyrOwner = _plyrOwner;
    }


}
