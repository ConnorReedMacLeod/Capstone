using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Discipline.DISCIPLINE;

public class ChrFischer : BaseChr {

    public ChrFischer(Chr _chrOwner) : base(_chrOwner) {

    }

    public override void SetName() {
        chrOwner.sName = "Fischer";
    }

    public override void SetDisciplines() {
        chrOwner.lstDisciplines = new List<Discipline.DISCIPLINE>() { TRAPPER, SCOUT };
    }

    //Defines all of a character's unique actions
    public override void SetInitialSkills() {

        chrOwner.arSkills[0] = new ActionHuntersQuarry(chrOwner);
        chrOwner.arSkills[1] = new ActionImpale(chrOwner);
        chrOwner.arSkills[2] = new ActionHarpoonGun(chrOwner);
        chrOwner.arSkills[3] = new ActionBucklerParry(chrOwner);

    }

}