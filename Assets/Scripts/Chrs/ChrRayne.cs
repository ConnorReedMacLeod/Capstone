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
    public override void SetInitialSkills() {

        chrOwner.arSkills[0] = new ActionCheerleader(chrOwner);
        chrOwner.arSkills[1] = new ActionCloudCushion(chrOwner);
        chrOwner.arSkills[2] = new ActionSpiritSlap(chrOwner);
        chrOwner.arSkills[3] = new ActionThunderStorm(chrOwner);

    }

}
