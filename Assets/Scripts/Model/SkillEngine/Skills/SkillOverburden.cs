using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillOverburden : Skill {

    public SkillOverburden(Chr _chrOwner) : base(_chrOwner) {

        sName = "Overburden";
        sDisplayName = "Overburden";

        typeUsage = new TypeUsageCantrip(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 0, 0, 0, 1));

        nCooldownInduced = 0;
        nFatigue = 0;

        InitTargets();

        lstSkillClauses = new List<ClauseSkillSelection>() {
            new Clause1(this),
        };
    }

    public override void InitTargets() {
        TarMana.AddTarget(this, manaCost);
        TarChr.AddTarget(this, TarChr.IsInPlay());
    }

    class Clause1 : ClauseSkillSelection {
        
        public SoulBurdened soulToCopy;

        public Clause1(Skill _skill) : base(_skill) {
            
            soulToCopy = new SoulBurdened(skill.chrOwner, null, skill);
        }

        public override string GetDescription() {

            return string.Format("Applies [Overburdened] to target Chr. [Overburdened]: Lowers Power by {0}", soulToCopy.nPowerDebuff);
        }

        public override void ClauseEffect(InputSkillSelection selections) {

            Chr chrSelected = (Chr)selections.lstSelections[1];

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(skill.chrOwner, chrSelected, new SoulBurdened(soulToCopy, chrSelected)) {
                sLabel = "Applying a burden"
            });
            

        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.OVERBURDEN;
    }

}