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
        chrOwner.lstDisciplines = new List<Discipline.DISCIPLINE>() { TESTING, FISCHER };
    }

    //Defines all of a character's unique skills
    public override void SetLoadoutSkills() {

        chrOwner.arSkillTypesOpeningLoadout[0] = SkillType.SKILLTYPE.BUNKER;
        chrOwner.arSkillTypesOpeningLoadout[1] = SkillType.SKILLTYPE.ADVANCE;
        chrOwner.arSkillTypesOpeningLoadout[2] = SkillType.SKILLTYPE.FIREBALL;
        chrOwner.arSkillTypesOpeningLoadout[3] = SkillType.SKILLTYPE.IMPALE;

    }

}