using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCheerleader : Skill {

    public SoulCheerleader soulPassive;

    public SkillCheerleader(Chr _chrOwner) : base(_chrOwner, 0) {//set the dominant clause 

        sName = "Cheerleader";
        sDisplayName = "Cheerleader";

        type = new TypePassive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 0 });

        nCooldownInduced = 0;
        nFatigue = 0;

        soulPassive = new SoulCheerleader(this.chrSource, this.chrSource, this);

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

            return string.Format("Initially applying Cheerleader on equip");
        }

        public override void ClauseEffect() {

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(skill.chrSource, skill.chrSource, ((SkillCheerleader)skill).soulPassive) {
                sLabel = skill.chrSource.sName + " is one peppy boi"
            });

        }

    };

    class ClauseUnequip : ClauseSpecial {

        public ClauseUnequip(Skill _skill) : base(_skill) {
            // Eventually add superficial tags here
        }

        public override string GetDescription() {

            return string.Format("Removing Cheerleader on unequip");
        }

        public override void ClauseEffect() {

            ContSkillEngine.PushSingleExecutable(new ExecRemoveSoulChr(skill.chrSource, ((SkillCheerleader)skill).soulPassive) {
                sLabel = skill.chrSource.sName + " is no longer peppy"
            });

        }

    };

    class Clause1 : ClauseSpecial {

        public Clause1(Skill _skill) : base(_skill) {
            // Eventually add superficial tags here
        }

        public override string GetDescription() {

            return string.Format("When {0} readies, all other allies gain {1} POWER until the end of turn.",
                skill.chrSource.sName, ((SkillCheerleader)skill).soulPassive.nPowerGain);
        }

        public override void ClauseEffect() {

            Debug.LogError("Shouldn't be executing a passive");

        }

    };
}
