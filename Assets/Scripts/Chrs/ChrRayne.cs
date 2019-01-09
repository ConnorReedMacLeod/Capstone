using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChrRayne : BaseChr {

    public ChrRayne(Chr _chrOwner) : base(_chrOwner) {

    }

    public override void SetName() {
        chrOwner.sName = "Rayne";
    }

    //Defines all of a character's unique actions
    public override void SetActions() {

        chrOwner.SetAction(0, new ActionCheerleader(chrOwner));
        chrOwner.SetAction(1, new ActionCloudCushion(chrOwner));
        chrOwner.SetAction(2, new ActionSpiritSlap(chrOwner));
        chrOwner.SetAction(3, new ActionStorm(chrOwner));
        
    }

}
