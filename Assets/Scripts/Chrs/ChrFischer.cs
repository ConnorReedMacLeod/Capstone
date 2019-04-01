using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChrFischer : BaseChr {

    public ChrFischer(Chr _chrOwner) : base(_chrOwner) {

    }

    public override void SetName() {
        chrOwner.sName = "Fischer";
    }



    //Defines all of a character's unique actions
    public override void SetActions() {
        chrOwner.SetAction(0, new ActionHuntersQuarry(chrOwner));
        chrOwner.SetAction(1, new ActionImpale(chrOwner));
        chrOwner.SetAction(2, new ActionHarpoonGun(chrOwner));
        chrOwner.SetAction(3, new ActionBucklerParry(chrOwner));

       
    }

}