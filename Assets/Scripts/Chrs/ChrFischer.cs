﻿using System.Collections;
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

        chrOwner.arActions[0] = new ActionFireball(chrOwner);
        chrOwner.arActions[1] = new ActionHeal(chrOwner);
        chrOwner.arActions[2] = new ActionExplosion(chrOwner);
        chrOwner.arActions[3] = new ActionFireball(chrOwner);

        chrOwner.arActions[7] = new ActionRest(chrOwner);
    }

}