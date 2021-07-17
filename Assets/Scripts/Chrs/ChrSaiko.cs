using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Discipline.DISCIPLINE;

public class ChrSaiko : BaseChr {

    public ChrSaiko(Chr _chrOwner) : base(_chrOwner) {

    }

    public override void SetName() {
        chrOwner.sName = "Saiko";
    }

    public override void SetDisciplines() {
        chrOwner.lstDisciplines = new List<Discipline.DISCIPLINE>() { TESTING, SAIKO };
    }

    //Defines all of a character's unique skills
    public override void SetLoadoutSkills() {

        chrOwner.arSkillTypesOpeningLoadout[0] = SkillType.SKILLTYPE.AMBUSH;
        chrOwner.arSkillTypesOpeningLoadout[1] = SkillType.SKILLTYPE.SMOKECOVER;
        chrOwner.arSkillTypesOpeningLoadout[2] = SkillType.SKILLTYPE.STICKYBOMB;
        chrOwner.arSkillTypesOpeningLoadout[3] = SkillType.SKILLTYPE.TRANQUILIZE;

    }

}