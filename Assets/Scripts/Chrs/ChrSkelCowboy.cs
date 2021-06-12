using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Discipline.DISCIPLINE;

public class ChrSkelCowboy : BaseChr {

    public ChrSkelCowboy(Chr _chrOwner) : base(_chrOwner) {

    }

    public override void SetName() {
        chrOwner.sName = "SkelCowboy";
    }

    public override void SetDisciplines() {
        chrOwner.lstDisciplines = new List<Discipline.DISCIPLINE>() { SCOUT, TRAPPER, GARDENER, GIANT };
    }

    //Defines all of a character's unique actions
    public override void SetLoadoutSkills() {

        chrOwner.arSkillTypesOpeningLoadout[0] = SkillType.SKILLTYPE.MULCH;
        chrOwner.arSkillTypesOpeningLoadout[1] = SkillType.SKILLTYPE.SURVEYTHELAND;
        chrOwner.arSkillTypesOpeningLoadout[2] = SkillType.SKILLTYPE.FLUSHOUT;
        chrOwner.arSkillTypesOpeningLoadout[3] = SkillType.SKILLTYPE.SNAPTRAP;

    }

}
