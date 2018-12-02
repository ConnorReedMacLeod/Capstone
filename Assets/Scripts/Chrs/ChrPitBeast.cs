using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChrPitBeast : BaseChr {

    public ChrPitBeast(Chr _chrOwner) : base(_chrOwner) {

    }

    public override void SetName() {
        chrOwner.sName = "PitBeast";
    }

    //Defines all of a character's unique actions
    public override void SetActions() {

        chrOwner.SetAction(0, new ActionSadism(chrOwner));
        chrOwner.SetAction(1, new ActionTendrilStab(chrOwner));
        chrOwner.SetAction(2, new ActionForcedEvolution(chrOwner));
        chrOwner.SetAction(3, new ActionTantrum(chrOwner));

        chrOwner.SetAction(7, new ActionRest(chrOwner));
    }

}