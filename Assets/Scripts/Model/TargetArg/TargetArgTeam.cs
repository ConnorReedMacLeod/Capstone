using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArgTeam : TargetArg {


    public Player plyrTar;
    public delegate bool funcLegalPlyr(Chr own, Player arg);
    public funcLegalPlyr fLegalCheck;

    public override void SetTarget(int indexTarget) {
        plyrTar = Player.arAllPlayers[indexTarget];
    }

    public TargetArgTeam(funcLegalPlyr _fLegalCheck) {
        fLegalCheck = _fLegalCheck;
    }

    public override bool CurrentlyLegal() {
        return fLegalCheck(chrOwner, plyrTar);
    }

    public override bool VerifyLegal(int indexTarget) {
        return fLegalCheck(chrOwner, Player.arAllPlayers[indexTarget]);
    }

    public override void Reset() {
        plyrTar = null;
    }

}
