using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBucklerParry : Skill {

    public SkillBucklerParry(Chr _chrOwner) : base(_chrOwner) {

        sName = "BucklerParry";
        sDisplayName = "Buckler Parry";

        typeUsage = new TypeUsageCantrip(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(1, 0, 0, 0, 0));

        nCooldownInduced = 8;
        nFatigue = 2;

        InitTargets();

        lstSkillClauses = new List<ClauseSkillSelection>() {
            new Clause1(this),
        };
    }

    public override void InitTargets() {
        TarMana.AddTarget(this, manaCost);
    }

    class Clause1 : ClauseSkillSelection {

        public SoulParry soulToCopy;

        public Clause1(Skill _skill) : base(_skill) {
            soulToCopy = new SoulParry(skill.chrOwner, null, skill);
        }

        public override string GetDescription() {

            return string.Format("Gain 15 DEFENSE and PARRY (4).\n" +
                "[PARRY]: When an enemy would deal damage to {0}, deal {1} damage to them and dispel.", skill.chrOwner.sName, soulToCopy.dmgCounterAttack.Get());
        }

        public override void ClauseEffect(InputSkillSelection selections) {

            Chr chrSelected = skill.chrOwner;

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(skill.chrOwner, chrSelected, new SoulParry(soulToCopy, chrSelected)));

        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.BUCKLERPARRY;
    }

}