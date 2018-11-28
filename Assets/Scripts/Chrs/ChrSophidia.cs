using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChrSophidia : BaseChr {

    public ChrSophidia(Chr _chrOwner) : base(_chrOwner) {

    }

    public override void SetName() {
        chrOwner.sName = "Sophidia";
    }

    //Defines all of a character's unique actions
    public override void SetActions() {

        chrOwner.arActions[0] = new ActionHiss(chrOwner);
        chrOwner.arActions[1] = new ActionVenemousBite(chrOwner);
        chrOwner.arActions[2] = new ActionSnakeLaunch(chrOwner);
        chrOwner.arActions[3] = new ActionHeal(chrOwner);

        chrOwner.arActions[7] = new ActionRest(chrOwner);
    }

}
