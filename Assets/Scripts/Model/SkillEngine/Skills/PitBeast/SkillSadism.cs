using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSadism : Skill {

    public SoulSadism soulPassive;

    public SkillSadism(Chr _chrOwner) : base(_chrOwner) {

        sName = "Sadism";
        sDisplayName = "Sadism";

        typeUsage = new TypeUsagePassive(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 0, 0, 0, 0));

        nCooldownInduced = 0;
        nFatigue = 0;

        InitTargets();

        soulPassive = new SoulSadism(this.chrOwner, this.chrOwner, this);

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

    public override void InitTargets() {
        //No targets to add
    }

    class ClauseEquip : Clause {

        public ClauseEquip(Skill _skill) : base(_skill) {
        }

        public override string GetDescription() {

            return string.Format("Initially applying Sadism on equip");
        }

        public override void ClauseEffect(Selections selections) {

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(skill.chrOwner, skill.chrOwner, ((SkillSadism)skill).soulPassive) {
                sLabel = "applying sadism"
            });

        }

    };

    class ClauseUnequip : Clause {

        public ClauseUnequip(Skill _skill) : base(_skill) {
        }

        public override string GetDescription() {

            return string.Format("Removing Sadism on unequip");
        }

        public override void ClauseEffect(Selections selections) {

            ContSkillEngine.PushSingleExecutable(new ExecRemoveSoulChr(skill.chrOwner, ((SkillSadism)skill).soulPassive) {
                sLabel = "removing sadism"
            });

        }

    };

    class Clause1 : Clause {

        public Clause1(Skill _skill) : base(_skill) { }

        public override string GetDescription() {

            return string.Format("When {0} would deal damage to a character with greater health, heal {1}.", skill.chrOwner.sName, ((SkillSadism)skill).soulPassive.heal.Get());
        }

        public override void ClauseEffect(Selections selections) {

            Debug.LogError("Shouldn't be executing a passive");

        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.SADISM;
    }
}
