﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRest : Skill {

    public SkillRest(Chr _chrOwner) : base(_chrOwner, 0) {

        sName = "Rest";
        sDisplayName = "Rest";

        type = new TypeCantrip(this);

        chrSource = _chrOwner;

        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 0 });

        nCooldownInduced = 0;
        nFatigue = 0;

        skillslot = null;

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : ClauseSpecial {

        public int nRestFatigue;

        public Clause1(Skill _skill) : base(_skill) {
            //TODO add in tags for base skills, and rest

            nRestFatigue = 3;
        }

        public override string GetDescription() {

            return string.Format("Finish this character's turn");
        }

        public override void ClauseEffect() {

            //Check if the character has any fatigue already
            if(skill.chrSource.nFatigue == 0) {
                //If not, then give them the rest fatigue
                ContSkillEngine.Get().AddExec(new ExecChangeFatigue(skill.chrSource, skill.chrSource, nRestFatigue, false) {
                    sLabel = "Resting"
                });
            }

            skill.chrSource.SetStateReadiness(new StateFatigued(skill.chrSource));

        }

    };

}
