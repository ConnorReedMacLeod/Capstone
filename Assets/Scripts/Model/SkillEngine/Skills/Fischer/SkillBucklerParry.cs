using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBucklerParry : Skill {

    public SkillBucklerParry(Chr _chrOwner) : base(_chrOwner) {

        sName = "BucklerParry";
        sDisplayName = "Buckler Parry";

        type = new TypeCantrip(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(1, 0, 0, 0, 0));

        nCooldownInduced = 8;
        nFatigue = 2;

        lstTargets = new List<Target>() {
            new TarMana(this, manaCost),
        };

        lstClauses = new List<Clause>() {
            new Clause1(this),
        };
    }

    class Clause1 : Clause {

        public SoulParry soulToCopy;

        public Clause1(Skill _skill) : base(_skill) {
            soulToCopy = new SoulParry(skill.chrOwner, null, skill);
        }

        public override string GetDescription() {

            return string.Format("Gain 15 DEFENSE and PARRY (4).\n" +
                "[PARRY]: When an enemy would deal damage to {0}, deal {1} damage to them and dispel.", skill.chrOwner.sName, soulToCopy.dmgCounterAttack.Get());
        }

        public override void ClauseEffect(Selections selections) {

            Chr chrSelected = (Chr)selections.lstSelections[0];

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(skill.chrOwner, chrSelected, new SoulParry(soulToCopy, chrSelected)));

        }

    };

}