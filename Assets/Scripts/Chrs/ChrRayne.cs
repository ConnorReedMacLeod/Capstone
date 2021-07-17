using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Discipline.DISCIPLINE;

public class ChrRayne : BaseChr {

    public ChrRayne(Chr _chrOwner) : base(_chrOwner) {

    }

    public override void SetName() {
        chrOwner.sName = "Rayne";
    }

    public override void SetDisciplines() {
        chrOwner.lstDisciplines = new List<Discipline.DISCIPLINE>() { TESTING, RAYNE };
    }

    //Defines all of a character's unique skills
    public override void SetLoadoutSkills() {

        chrOwner.arSkillTypesOpeningLoadout[0] = SkillType.SKILLTYPE.CHEERLEADER;
        chrOwner.arSkillTypesOpeningLoadout[1] = SkillType.SKILLTYPE.CLOUDCUSHION;
        chrOwner.arSkillTypesOpeningLoadout[2] = SkillType.SKILLTYPE.SPIRITSLAP;
        chrOwner.arSkillTypesOpeningLoadout[3] = SkillType.SKILLTYPE.THUNDERSTORM;
    }

}
