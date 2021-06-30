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
        chrOwner.lstDisciplines = new List<Discipline.DISCIPLINE>() { TESTING, PITBEAST };
    }

    //Defines all of a character's unique skills
    public override void SetLoadoutSkills() {

        chrOwner.arSkillTypesOpeningLoadout[0] = SkillType.SKILLTYPE.FORCEDEVOLUTION;
        chrOwner.arSkillTypesOpeningLoadout[1] = SkillType.SKILLTYPE.SADISM;
        chrOwner.arSkillTypesOpeningLoadout[2] = SkillType.SKILLTYPE.TANTRUM;
        chrOwner.arSkillTypesOpeningLoadout[3] = SkillType.SKILLTYPE.TENDRILSTAB;
    }

}