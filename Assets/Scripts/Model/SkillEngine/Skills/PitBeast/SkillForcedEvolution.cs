using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillForcedEvolution : Skill {

    public SkillForcedEvolution(Chr _chrOwner) : base(_chrOwner) {

        sName = "ForcedEvolution";
        sDisplayName = "Forced Evolution";

        typeUsage = new TypeUsageActive(this);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 0, 0, 1, 0));

        nCooldownInduced = 6;
        nFatigue = 1;

        lstTargets = new List<Target>() {
            new TarMana(this, manaCost),
        };

        lstClauses = new List<Clause>() {
            new Clause1(this),
        };
    }

    class Clause1 : Clause {

        public int nLifeLoss = 5;

        public Clause1(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {

            return string.Format("Lose {0} life.", nLifeLoss);
        }

        public override void ClauseEffect(Selections selections) {

            Chr chrSelected = skill.chrOwner;

            ContSkillEngine.PushSingleExecutable(new ExecLoseLife(skill.chrOwner, chrSelected, nLifeLoss) {
                sLabel = "It's going berserk"
            });

        }

    };

    class Clause2 : Clause {

        public SoulEvolved soulToCopy;

        public Clause2(Skill _skill) : base(_skill) {

            soulToCopy = new SoulEvolved(skill.chrOwner, null, skill);
        }

        public override string GetDescription() {

            return string.Format("Gain {0} POWER.", soulToCopy.nPowerBuff);
        }

        public override void ClauseEffect(Selections selections) {

            Chr chrSelected = skill.chrOwner;

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(skill.chrOwner, chrSelected, new SoulEvolved(soulToCopy, chrSelected)) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("PitBeast/sndForcedEvolution", 4.667f) },
                sLabel = "It's evolving"
            });

        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.FORCEDEVOLUTION;
    }
}
