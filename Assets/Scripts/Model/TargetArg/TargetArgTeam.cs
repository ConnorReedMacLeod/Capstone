using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArgTeam : TargetArg {


    public delegate bool funcLegalPlyr(Chr own, Player arg);
    public funcLegalPlyr fLegalCheck;

    public TargetArgTeam(funcLegalPlyr _fLegalCheck) {
        fLegalCheck = _fLegalCheck;
    }


    public override bool WouldBeLegal(int indexTarget) {
        if (indexTarget >= Player.MAXPLAYERS) {
            Debug.LogError("Trying to select a player with index " + indexTarget + " that doesn't exist");
            return false;
        }

        return fLegalCheck(chrOwner, Player.lstAllPlayers[indexTarget]);
    }

}
