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
        chrOwner.lstDisciplines = new List<Discipline.DISCIPLINE>() { GIANT, TRAPPER, FALCONER, GARDENER };
    }

    //Defines all of a character's unique actions
    public override void SetLoadoutSkills() {

        chrOwner.arSkillTypesOpeningLoadout[0] = SkillType.SKILLTYPE.RECON;
        chrOwner.arSkillTypesOpeningLoadout[1] = SkillType.SKILLTYPE.FLUSHOUT;
        chrOwner.arSkillTypesOpeningLoadout[2] = SkillType.SKILLTYPE.SNAPTRAP;
        chrOwner.arSkillTypesOpeningLoadout[3] = SkillType.SKILLTYPE.MULCH;

    }

}
