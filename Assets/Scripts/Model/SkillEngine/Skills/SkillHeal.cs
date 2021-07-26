using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHeal : Skill {

    public Healing heal;
    public int nBaseHealing;

    public SkillHeal(Chr _chrOwner) : base(_chrOwner, 0) { //Set the dominant clause to be the first clause

        sName = "Heal";
        sDisplayName = "Heal";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 1, 0 });

        nCooldownInduced = 3;
        nFatigue = 3;

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };

    }

    class Clause1 : ClauseChr {

        int nBaseHealing = 5;
        Healing heal;

        public Clause1(Skill _skill) : base(_skill) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrRanged(this), //Base Tag always goes first
                new ClauseTagChrAlly(this)
            });

            //Create and store a copy of the intended healing effect so that any information/effects
            // can be updated accurately
            heal = new Healing(skill.chrOwner, null, nBaseHealing);

        }

        public override string GetDescription() {

            //TODO - eventually figure out how I'm gonna dynamically generate the text targets
            return string.Format("Heal {0} life to an Ally", heal.Get());
        }

        public override void ClauseEffect(Chr chrSelected) {

            //Push an executable with this skill's owner as the source, the selected character as the target,
            // and we can copy the stored healing instance to apply
            ContSkillEngine.PushSingleExecutable(new ExecHeal(skill.chrOwner, chrSelected, heal) {
                sLabel = "Healing"
            });

        }

    };
}
