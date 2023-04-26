using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRest : Skill {

    public SkillRest(Chr _chrOwner) : base(_chrOwner) {

        sName = "Rest";
        sDisplayName = "Rest";

        typeUsage = new TypeUsageCantrip(this);

        chrOwner = _chrOwner;

        manaCost = new ManaCost(new Mana(0, 0, 0, 0, 0));

        nCooldownInduced = 0;
        nFatigue = 0;

        skillslot = null;

        InitTargets();

        lstSkillClauses = new List<ClauseSkillSelection>() {
            new Clause1(this)
        };
    }

    public override void InitTargets() {
        //No targetting required for a rest action
    }

    class Clause1 : ClauseSkillSelection {

        public int nRestFatigue;

        public Clause1(Skill _skill) : base(_skill) {

            nRestFatigue = 3;
        }

        public override string GetDescription() {

            return string.Format("Finish this character's turn");
        }

        public override void ClauseEffect(InputSkillSelection selections) {

            //Check if the character has any fatigue already
            if(skill.chrOwner.nFatigue == 0) {
                //If not, then give them the rest fatigue
                ContSkillEngine.Get().AddExec(new ExecChangeFatigue(skill.chrOwner, skill.chrOwner, nRestFatigue) {
                    sLabel = "Resting"
                });
            }

            skill.chrOwner.SetStateReadiness(new StateFatigued(skill.chrOwner));

        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.REST;
    }

}
