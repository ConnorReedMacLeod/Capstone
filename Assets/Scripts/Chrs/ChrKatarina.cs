using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChrKatarina : BaseChr {

    public ChrKatarina(Chr _chrOwner) : base(_chrOwner) {

    }

    public override void SetName() {
        chrOwner.sName = "Katarina";
    }

    //Defines all of a character's unique actions
    public override void SetActions() {

        chrOwner.arActions[0] = new ActionFortissimo(chrOwner);
        chrOwner.arActions[1] = new ActionReverberate(chrOwner);
        chrOwner.arActions[2] = new ActionSerenade(chrOwner);
        chrOwner.arActions[3] = new ActionCacaphony(chrOwner);

        chrOwner.arActions[7] = new ActionRest(chrOwner);
    }

}
