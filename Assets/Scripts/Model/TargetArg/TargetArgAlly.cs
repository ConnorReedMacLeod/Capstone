using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArgAlly : TargetArgChr {

    public TargetArgAlly(funcLegalChr _fLegalCheck) : base (_fLegalCheck) {

    }

    public override bool CurrentlyLegal() {
        if(chrTar == null) {
            Debug.Log("Bad Target - No target set");
            return false;
        } else if (chrOwner.plyrOwner != chrTar.plyrOwner) {
            Debug.Log("Bad Target - You need to target an allied character");
            return false;
        } else if (chrTar.bDead == true) {
            Debug.Log("Bad Target - You can't target a dead character");
            return false;
        }

        //Try the base checks for any character targetting
        return base.CurrentlyLegal();
    }

    public override bool VerifyLegal(int indexTarget) {
        if (chrOwner.plyrOwner != Chr.arAllChrs[indexTarget].plyrOwner) {
            Debug.Log("Bad Target - You need to target an allied character");
            return false;
        } else if (Chr.arAllChrs[indexTarget].bDead == true) {
            Debug.Log("Bad Target - You can't target a dead character");
            return false;
        }

        //Try the base checks for any character targetting
        return base.VerifyLegal(indexTarget);
    }
}
