using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChrSkelCowboy : BaseChr {

    public ChrSkelCowboy(Chr _chrOwner) : base(_chrOwner) {

    }

    public override void SetName() {
        chrOwner.sName = "SkelCowboy";
    }

    //Defines all of a character's unique actions
    public override void SetInitialSkills() {

        chrOwner.arSkills[0] = new ActionFireball(chrOwner);
        chrOwner.arSkills[1] = new ActionHeal(chrOwner);
        chrOwner.arSkills[2] = new ActionExplosion(chrOwner);
        chrOwner.arSkills[3] = new ActionExplosion(chrOwner);

    }

}
