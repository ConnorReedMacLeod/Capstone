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
        chrOwner.lstDisciplines = new List<Discipline.DISCIPLINE>() { GARDENER, GIANT };
    }

    //Defines all of a character's unique skills
    public override void SetLoadoutSkills() {

        chrOwner.arSkillTypesOpeningLoadout[0] = SkillType.SKILLTYPE.MULCH;
        chrOwner.arSkillTypesOpeningLoadout[1] = SkillType.SKILLTYPE.PLANTSUNFLOWER;
        chrOwner.arSkillTypesOpeningLoadout[2] = SkillType.SKILLTYPE.SURVEYTHELAND;
        chrOwner.arSkillTypesOpeningLoadout[3] = SkillType.SKILLTYPE.PLANTSUNFLOWER;
    }

}
