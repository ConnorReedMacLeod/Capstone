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

        chrOwner.SetAction(0, new ActionHiss(chrOwner));
        chrOwner.SetAction(1, new ActionVenomousBite(chrOwner));
        chrOwner.SetAction(2, new ActionTwinSnakes(chrOwner));
        chrOwner.SetAction(3, new ActionRegenerate(chrOwner));
        
    }

}
