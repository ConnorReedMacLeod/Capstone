using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArgAlly : TargetArgChr {

    public TargetArgAlly(funcLegalChr _fLegalCheck) : base (_fLegalCheck) {

    }

    public override bool WouldBeLegal(int indexTarget) {
        if (indexTarget >= Chr.arAllChrs.Length) {
            Debug.LogError("Trying to select a character with index " + indexTarget + " that doesn't exist");
            return false;
        }else if (chrOwner.plyrOwner != Chr.arAllChrs[indexTarget].plyrOwner) {
            Debug.Log("Bad Target - You need to target an allied character");
            return false;
        } else if (Chr.arAllChrs[indexTarget].bDead == true) {
            Debug.Log("Bad Target - You can't target a dead character");
            return false;
        }

        //Try the base checks for any character targetting
        return base.WouldBeLegal(indexTarget);
    }
}
