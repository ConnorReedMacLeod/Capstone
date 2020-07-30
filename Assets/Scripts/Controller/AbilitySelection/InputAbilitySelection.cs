﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputAbilitySelection : MonoBehaviour {

    public Player plyrOwner;

    //The stored information for what targetting information we'll give
    public int nSelectedChrId;
    public int nSelectedAbility;
    public int[] arTargetIndices;

    public void ResetTargets() {
        nSelectedChrId = -1;
        nSelectedAbility = -1;
        arTargetIndices = null;
    }

    public abstract void StartSelection();

    public abstract void GaveInvalidTarget();

    public void SetOwner(Player _plyrOwner) {
        plyrOwner = _plyrOwner;
    }

    //TODONOW - this can store temporary information in it's nSelectedChrId/Ability fields, but then 
    //          formally bind this into a SelectionInfo when the targetting is finished so that it can be used by outside sources

}
