using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillKill : Skill {

    public SkillKill(Chr _chrOwner) : base(_chrOwner) {

        sName = "Kill";
        sDisplayName = "Kill";

        typeUsage = new TypeUsageActive(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 0, 0, 1, 0));

        nCooldownInduced = 6;
        nFatigue = 4;

        InitTargets();

        lstSkillClauses = new List<ClauseSkillSelection>() {
            new Clause1(this)
        };
    }

    public override void InitTargets() {
        TarMana.AddTarget(this, manaCost);
        TarChr.AddTarget(this, Target.AND(TarChr.IsInPlay(), TarChr.IsDiffTeam(chrOwner)));
    }

    class Clause1 : ClauseSkillSelection {

        public Clause1(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {

            return string.Format("Set an enemy's health to 0");
        }

        public override void ClauseEffect(InputSkillSelection selections) {

            Chr chrSelected = (Chr)selections.lstSelections[1];

            ContSkillEngine.PushSingleExecutable(new ExecSetHealth (skill.chrOwner, chrSelected, 0) {
                sLabel = "Setting to 0 health"
            });

        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.KILL;
    }

}
