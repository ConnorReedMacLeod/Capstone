using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArgAlly : TargetArgChr {

    public TargetArgAlly(funcLegalChr _fLegalCheck) : base (_fLegalCheck) {

    }

    public override bool VerifyLegal() {
        if (chrOwner.plyrOwner != chrTar.plyrOwner) {
            Debug.Log("Bad Target - You need to target an allied character");
            return false;
        } else if (chrTar.bDead == true) {
            Debug.Log("Bad Target - You can't target a dead character");
            return false;
        } else if (!fLegalCheck(chrOwner, chrTar)) {
            Debug.Log("Bad Target - " + chrTar.sName + " does not meet this ability's specifications");
            return false;
        }

        return true;
    }
}
