using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRetreat : Skill {

    public SkillRetreat(Chr _chrOwner) : base(_chrOwner) {

        sName = "Retreat";
        sDisplayName = "Retreat";

        typeUsage = new TypeUsageActive(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 0, 0, 0, 1), true);

        nCooldownInduced = 4;
        nFatigue = 2;

        InitTargets();

        lstSkillClauses = new List<ClauseSkillSelection>() {
            new Clause1(this)
        };
    }

    public override void InitTargets() {
        TarMana.AddTarget(this, manaCost);
        TarChr.AddTarget(this, Target.AND(TarChr.IsSameTeam(chrOwner), TarChr.IsBenched()));
    }

    class Clause1 : ClauseSkillSelection {

        public Clause1(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {

            return string.Format("Switch with target benched Ally");
        }

        public override void ClauseEffect(InputSkillSelection selections) {

            Chr chrSwappingWith = (Chr)selections.lstSelections[1];
            Debug.Log("Switching with " + chrSwappingWith);

            ContSkillEngine.PushSingleExecutable(new ExecSwitchCharWithChar(skill.chrOwner, skill.chrOwner, chrSwappingWith) {
                sLabel = "Swapping to bench"
            });

        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.RETREAT;
    }

}
