using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCheerleader : Skill {

    public SoulCheerleader soulPassive;

    public SkillCheerleader(Chr _chrOwner) : base(_chrOwner) {

        sName = "Cheerleader";
        sDisplayName = "Cheerleader";

        type = new TypePassive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 0 });

        nCooldownInduced = 0;
        nFatigue = 0;

        soulPassive = new SoulCheerleader(this.chrOwner, this.chrOwner, this);

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

    class ClauseEquip : Clause {

        public ClauseEquip(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {

            return string.Format("Initially applying Cheerleader on equip");
        }

        public override void ClauseEffect(Selections selections) {

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(skill.chrOwner, skill.chrOwner, ((SkillCheerleader)skill).soulPassive) {
                sLabel = skill.chrOwner.sName + " is one peppy boi"
            });

        }

    };

    class ClauseUnequip : Clause {

        public ClauseUnequip(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {

            return string.Format("Removing Cheerleader on unequip");
        }

        public override void ClauseEffect(Selections selections) {

            ContSkillEngine.PushSingleExecutable(new ExecRemoveSoulChr(skill.chrOwner, ((SkillCheerleader)skill).soulPassive) {
                sLabel = skill.chrOwner.sName + " is no longer peppy"
            });

        }

    };

    class Clause1 : Clause {

        public Clause1(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {

            return string.Format("When {0} readies, all other allies gain {1} POWER until the end of turn.",
                skill.chrOwner.sName, ((SkillCheerleader)skill).soulPassive.nPowerGain);
        }

        public override void ClauseEffect(Selections selections) {

            Debug.LogError("Shouldn't be executing a passive");

        }

    };
}
