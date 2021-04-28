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
    public override void SetInitialSkills() {

        chrOwner.arSkills[0] = new ActionFortissimo(chrOwner);
        chrOwner.arSkills[1] = new ActionLeech(chrOwner);
        chrOwner.arSkills[2] = new ActionSerenade(chrOwner);
        chrOwner.arSkills[3] = new ActionCacophony(chrOwner);
    }

}
