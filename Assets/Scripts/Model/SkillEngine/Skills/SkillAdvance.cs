using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAdvance : Skill {

    public SkillAdvance(Chr _chrOwner) : base(_chrOwner) {

        sName = "Advance";
        sDisplayName = "Advance";

        typeUsage = new TypeUsageActive(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(1, 0, 0, 0, 0));

        nCooldownInduced = 6;
        nFatigue = 4;

        InitTargets();

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    public override void InitTargets() {
        TarMana.AddTarget(this, manaCost);
        TarPosition.AddTarget(this, Target.AND(TarPosition.IsFrontline(), TarPosition.IsSameTeam(chrOwner)));
    }

    class Clause1 : Clause {


        public Clause1(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {

            return string.Format("Switch to target Allied Frontline Position");
        }

        public override void ClauseEffect(Selections selections) {
            Chr chrSelected = skill.chrOwner;

            Position posToSwitchTo = (Position)selections.lstSelections[1];

            ContSkillEngine.PushSingleExecutable(new ExecSwitchChar(skill.chrOwner, chrSelected, posToSwitchTo) {
                sLabel = "I'll lead the way"
            });

        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.ADVANCE;
    }


}
