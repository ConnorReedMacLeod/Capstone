using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChrSaiko : BaseChr {

    public ChrSaiko(Chr _chrOwner) : base(_chrOwner) {

    }

    public override void SetName() {
        chrOwner.sName = "Saiko";
    }

    //Defines all of a character's unique actions
    public override void SetActions() {

        chrOwner.SetAction(0, new ActionAmbush(chrOwner));
        chrOwner.SetAction(1, new ActionSmokeCover(chrOwner));
        chrOwner.SetAction(2, new ActionTranquilize(chrOwner));
        chrOwner.SetAction(3, new ActionStickyBomb(chrOwner));
        
    }

}