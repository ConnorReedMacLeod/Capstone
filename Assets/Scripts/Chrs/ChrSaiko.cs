﻿using System.Collections;
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
        chrOwner.lstDisciplines = new List<Discipline.DISCIPLINE>() { FALCONER, SCOUT };
    }

    //Defines all of a character's unique actions
    public override void SetInitialSkills() {

        chrOwner.arSkills[0] = new ActionAmbush(chrOwner);
        chrOwner.arSkills[1] = new ActionSmokeCover(chrOwner);
        chrOwner.arSkills[2] = new ActionTranquilize(chrOwner);
        chrOwner.arSkills[3] = new ActionStickyBomb(chrOwner);

    }

}