using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillThunderStorm : Skill {

    public SkillThunderStorm(Chr _chrOwner) : base(_chrOwner) {

        sName = "ThunderStorm";
        sDisplayName = "Thunder Storm";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 1, 1, 0, 0 });

        nCooldownInduced = 10;
        nFatigue = 5;

        lstTargets = new List<Target>() {

        };

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : Clause {

        Damage dmg;
        public int nBaseDamage = 15;
        public int nBaseStun = 2;

        public Clause1(Skill _skill) : base(_skill) {
            dmg = new Damage(skill.chrOwner, null, nBaseDamage);
        }

        public override string GetDescription() {

            return string.Format("Deal {0} damage and {1} fatigue to all characters on the target character's team", dmg.Get(), nBaseStun);
        }

        public override void ClauseEffect(Selections selections) {

            Chr chrSelected = (Chr)selections.lstSelections[0];

            ContSkillEngine.PushSingleExecutable(new ExecStun(skill.chrOwner, chrSelected, nBaseStun) {
                sLabel = "Crackle Crackle"
            });

            ContSkillEngine.PushSingleExecutable(new ExecDealDamage(skill.chrOwner, chrSelected, dmg) {
                sLabel = "Caught in the storm"
            });



        }

    };

}