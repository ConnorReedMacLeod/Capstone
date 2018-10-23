using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionExplosion : Action {



    public ActionExplosion(Chr _chrOwner) : base(1, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        arArgs[0] = new TargetArgTeam((own, tar) => true); // any team selection is fine

        sName = "Explosion";
        type = ActionType.ACTIVE;

        //Physical, Mental, Energy, Blood, Effort
        arCost = new int[] { 0, 0, 2, 0, 1 };

        nCd = 10;
        nRecharge = 6;

        sDescription = "Deal 5 damage to all characters on target team";

        SetArgOwners();
    }

    override public void Execute() {
        //It's a bit awkward that you have to do this typecasting, 
        // but at least it's eliminated from the targetting lambda
        Player tar = ((TargetArgTeam)arArgs[0]).plyrTar;

        Debug.Log("Player " + tar.id + " has been exploded");

        for(int i=0; i<tar.nChrs; i++) {
            tar.arChr[i].ChangeHealth(-5);
        }

        //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;


        base.Execute();
    }

}
