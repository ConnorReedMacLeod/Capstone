using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSadism : Skill {

    public SoulSadism soulPassive;

    public SkillSadism(Chr _chrOwner) : base(_chrOwner, 0) {// set the dominant clause to 0

        sName = "Sadism";
        sDisplayName = "Sadism";

        type = new TypePassive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 0 });

        nCooldownInduced = 0;
        nFatigue = 0;


        soulPassive = new SoulSadism(this.chrSource, this.chrSource, this);

        lstClausesOnEquip = new List<Clause>() {
            new ClauseEquip(this)
        };

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };

        lstClausesOnUnequip = new List<Clause>() {
            new ClauseUnequip(this)
        };
    }

    class ClauseEquip : ClauseSpecial {

        public ClauseEquip(Skill _skill) : base(_skill) {
            // Eventually add superficial tags here
        }

        public override string GetDescription() {

            return string.Format("Initially applying Sadism on equip");
        }

        public override void ClauseEffect() {

            ContSkillEngine.PushSingleExecutable(new ExecApplySoul(skill.chrSource, skill.chrSource, ((SkillSadism)skill).soulPassive) {
                sLabel = "applying sadism"
            });

        }

    };

    class ClauseUnequip : ClauseSpecial {

        public ClauseUnequip(Skill _skill) : base(_skill) {
            // Eventually add superficial tags here
        }

        public override string GetDescription() {

            return string.Format("Removing Sadism on unequip");
        }

        public override void ClauseEffect() {

            ContSkillEngine.PushSingleExecutable(new ExecRemoveSoul(skill.chrSource, ((SkillSadism)skill).soulPassive) {
                sLabel = "removing sadism"
            });

        }

    };

    class Clause1 : ClauseSpecial {

        public Clause1(Skill _skill) : base(_skill) {
            // Eventually add superficial tags here
        }

        public override string GetDescription() {

            return string.Format("When {0} would deal damage to a character with greater health, heal {1}.", skill.chrSource.sName, ((SkillSadism)skill).soulPassive.heal.Get());
        }

        public override void ClauseEffect() {

            Debug.LogError("Shouldn't be executing a passive");

        }

    };
}
