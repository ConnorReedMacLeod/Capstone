using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBucklerParry : Skill {

    public SkillBucklerParry(Chr _chrOwner) : base(_chrOwner, 0) {// 0 is the dominant clause

        sName = "BucklerParry";
        sDisplayName = "Buckler Parry";

        type = new TypeCantrip(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 1, 0, 0, 0, 0 });

        nCooldownInduced = 8;
        nFatigue = 2;

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : ClauseChr {

        public SoulParry soulToCopy;

        public Clause1(Skill _skill) : base(_skill) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrRanged(this), //Base Tag always goes first
                new ClauseTagChrSelf(this)
            });

            soulToCopy = new SoulParry(skill.chrOwner, null, skill);
        }

        public override string GetDescription() {

            return string.Format("Gain 15 DEFENSE and PARRY (4).\n" +
                "[PARRY]: When an enemy would deal damage to {0}, deal {1} damage to them and dispel.", skill.chrOwner.sName, soulToCopy.dmgCounterAttack.Get());
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(skill.chrOwner, chrSelected, new SoulParry(soulToCopy, chrSelected)));

        }

    };

}