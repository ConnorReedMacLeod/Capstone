using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRest : Skill {

    public SkillRest(Chr _chrOwner) : base(_chrOwner) {

        sName = "Rest";
        sDisplayName = "Rest";

        type = new TypeCantrip(this);

        chrOwner = _chrOwner;

        manaCost = new ManaCost(new Mana(0, 0, 0, 0, 0));

        nCooldownInduced = 0;
        nFatigue = 0;

        skillslot = null;

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : Clause {

        public int nRestFatigue;

        public Clause1(Skill _skill) : base(_skill) {

            nRestFatigue = 3;
        }

        public override string GetDescription() {

            return string.Format("Finish this character's turn");
        }

        public override void ClauseEffect(Selections selections) {

            //Check if the character has any fatigue already
            if(skill.chrOwner.nFatigue == 0) {
                //If not, then give them the rest fatigue
                ContSkillEngine.Get().AddExec(new ExecChangeFatigue(skill.chrOwner, skill.chrOwner, nRestFatigue, false) {
                    sLabel = "Resting"
                });
            }

            skill.chrOwner.SetStateReadiness(new StateFatigued(skill.chrOwner));

        }

    };

}
