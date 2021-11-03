using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillStrategize : Skill {

    public SkillStrategize(Chr _chrOwner) : base(_chrOwner) {

        sName = "Strategize";
        sDisplayName = "Strategize";

        typeUsage = new TypeUsageActive(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 1, 0, 0, 0), true);

        nCooldownInduced = 6;
        nFatigue = 4;

        InitTargets();

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    public override void InitTargets() {
        TarMana.AddTarget(this, manaCost);
        TarAdapt.AddTarget(this, TarSkillSlot.IsOwnedBySameChr(chrOwner), Target.TRUE);
    }

    class Clause1 : Clause {

        public Clause1(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {

            return string.Format("Adapt target Skill on this Character");
        }

        public override void ClauseEffect(Selections selections) {

            SkillSlot ssAdaptingFrom = (SkillSlot)selections.lstSelections[1];
            SkillType.SKILLTYPE skilltypeAdaptingTo = (SkillType.SKILLTYPE)selections.lstSelections[2];
            Debug.Log("Adapting to " + skilltypeAdaptingTo);
            
            ContSkillEngine.PushSingleExecutable(new ExecAdaptSkill(skill.chrOwner, ssAdaptingFrom, skilltypeAdaptingTo) { 
                sLabel = "Adapting skill"
            });

        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.STRATEGIZE;
    }

}
