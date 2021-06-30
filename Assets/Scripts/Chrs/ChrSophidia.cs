using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Discipline.DISCIPLINE;

public class ChrSophidia : BaseChr {

    public ChrSophidia(Chr _chrOwner) : base(_chrOwner) {

    }

    public override void SetName() {
        chrOwner.sName = "Sophidia";
    }

    public override void SetDisciplines() {
        chrOwner.lstDisciplines = new List<Discipline.DISCIPLINE>() { TESTING, SOPHIDIA };
    }

    //Defines all of a character's unique skills
    public override void SetLoadoutSkills() {

        chrOwner.arSkillTypesOpeningLoadout[0] = SkillType.SKILLTYPE.HISS;
        chrOwner.arSkillTypesOpeningLoadout[1] = SkillType.SKILLTYPE.HYDRASREGEN;
        chrOwner.arSkillTypesOpeningLoadout[2] = SkillType.SKILLTYPE.TWINSNAKES;
        chrOwner.arSkillTypesOpeningLoadout[3] = SkillType.SKILLTYPE.VENEMOUSBITE;

    }

}
