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
    public override void SetInitialSkills() {

        chrOwner.arSkills[0] = new ActionHiss(chrOwner);
        chrOwner.arSkills[1] = new ActionVenomousBite(chrOwner);
        chrOwner.arSkills[2] = new ActionTwinSnakes(chrOwner);
        chrOwner.arSkills[3] = new ActionHydrasRegen(chrOwner);

    }

}
