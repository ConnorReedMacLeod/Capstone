using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Discipline.DISCIPLINE;

public class ChrKatarina : BaseChr {

    public ChrKatarina(Chr _chrOwner) : base(_chrOwner) {

    }

    public override void SetName() {
        chrOwner.sName = "Katarina";
    }

    public override void SetDisciplines() {
        chrOwner.lstDisciplines = new List<Discipline.DISCIPLINE>() { GARDENER, SCOUT };
    }

    //Defines all of a character's unique actions
    public override void SetLoadoutSkills() {

        chrOwner.arSkillTypesOpeningLoadout[0] = SkillType.SKILLTYPE.RECON;
        chrOwner.arSkillTypesOpeningLoadout[1] = SkillType.SKILLTYPE.PLANTSUNFLOWER;
        chrOwner.arSkillTypesOpeningLoadout[2] = SkillType.SKILLTYPE.SURVEYTHELAND;
        chrOwner.arSkillTypesOpeningLoadout[3] = SkillType.SKILLTYPE.RECON;
    }

}
