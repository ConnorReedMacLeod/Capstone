using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCheerleader : Skill {

    public SoulCheerleader soulPassive;

    public SkillCheerleader(Chr _chrOwner) : base(_chrOwner) {

        sName = "Cheerleader";
        sDisplayName = "Cheerleader";

        typeUsage = new TypeUsagePassive(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 0, 0, 0, 0));

        nCooldownInduced = 0;
        nFatigue = 0;

        InitTargets();

        soulPassive = new SoulCheerleader(this.chrOwner, this.chrOwner, this);

        lstClausesOnEquip = new List<ClauseSkill>() {
            new ClauseEquip(this)
        };

        lstSkillClauses = new List<ClauseSkillSelection>() {
            new Clause1(this)
        };

        lstClausesOnUnequip = new List<ClauseSkill>() {
            new ClauseUnequip(this)
        };
    }

    public override void InitTargets() {
        //No targets needed for a passive
    }

    class ClauseEquip : ClauseSkill {

        public ClauseEquip(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {

            return string.Format("Initially applying Cheerleader on equip");
        }

        public override void Execute() {

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(skill.chrOwner, skill.chrOwner, ((SkillCheerleader)skill).soulPassive) {
                sLabel = skill.chrOwner.sName + " is one peppy boi"
            });

        }

    };

    class ClauseUnequip : ClauseSkill {

        public ClauseUnequip(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {

            return string.Format("Removing Cheerleader on unequip");
        }

        public override void Execute() {

            ContSkillEngine.PushSingleExecutable(new ExecRemoveSoulChr(skill.chrOwner, ((SkillCheerleader)skill).soulPassive) {
                sLabel = skill.chrOwner.sName + " is no longer peppy"
            });

        }

    };

    class Clause1 : ClauseSkillSelection {

        public Clause1(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {

            return string.Format("When {0} readies, all other allies gain {1} POWER until the end of turn.",
                skill.chrOwner.sName, ((SkillCheerleader)skill).soulPassive.nPowerGain);
        }

        public override void ClauseEffect(InputSkillSelection selections) {

            Debug.LogError("Shouldn't be executing a passive");

        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.CHEERLEADER;
    }
}
