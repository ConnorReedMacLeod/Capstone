using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArgTeam : TargetArg {


    public Player plyrTar;
    public delegate bool funcLegalPlyr(Chr own, Player arg);
    public funcLegalPlyr fLegalCheck;

    //WARNING: This feels like it should be shared among TargetArgs but it isn't
    public bool setTar(Player _plyrTar) {
        Player plyrOldTar = _plyrTar;
        plyrTar = _plyrTar;
        if (VerifyLegal()) {
            return true; //the targetting was successful
        } else {
            plyrTar = plyrOldTar;
            return false; //bad target
        }

    }

    public TargetArgTeam(funcLegalPlyr _fLegalCheck) {
        fLegalCheck = _fLegalCheck;
    }

    public override bool VerifyLegal() {
        return fLegalCheck(chrOwner, plyrTar);
    }

    public override void Reset() {
        plyrTar = null;
    }

}
