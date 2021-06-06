using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Discipline.DISCIPLINE;

public class ChrPitBeast : BaseChr {

    public ChrPitBeast(Chr _chrOwner) : base(_chrOwner) {

    }

    public override void SetName() {
        chrOwner.sName = "PitBeast";
    }

    public override void SetDisciplines() {
        chrOwner.lstDisciplines = new List<Discipline.DISCIPLINE>() { GIANT, TRAPPER };
    }

    //Defines all of a character's unique actions
    public override void SetInitialSkills() {

        chrOwner.arSkills[0] = new ActionSadism(chrOwner);
        chrOwner.arSkills[1] = new ActionTendrilStab(chrOwner);
        chrOwner.arSkills[2] = new ActionForcedEvolution(chrOwner);
        chrOwner.arSkills[3] = new ActionTantrum(chrOwner);
    }

}