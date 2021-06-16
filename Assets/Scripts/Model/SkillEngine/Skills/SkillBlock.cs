using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBlock : Skill {

    public SkillBlock(Chr _chrOwner) : base(_chrOwner, 0) {

        sName = "DeclareBlocker";
        sDisplayName = "Declare Vanguard";

        type = new TypeCantrip(this);

        skillslot = null;

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 0 });

        nCooldownInduced = 1; //This might have issues if you can reduce cooldowns a lot - don't want looping
        nFatigue = 0;

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : ClauseSpecial {

        public Clause1(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {

            //TODO - eventually figure out how I'm gonna dynamically generate the text targets
            return "Become the Vanguard.  (Melee attacks can only target the Vanguard)";
        }

        public override void ClauseEffect() {
            ContSkillEngine.PushSingleExecutable(new ExecBecomeBlocker(skill.chrSource, skill.chrSource) {
                sLabel = "Becoming the vanguard"
            });
        }

    }

}