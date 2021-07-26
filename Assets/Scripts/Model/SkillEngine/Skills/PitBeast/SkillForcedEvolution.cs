using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillForcedEvolution : Skill {

    public SkillForcedEvolution(Chr _chrOwner) : base(_chrOwner, 0) {//Set the dominant clause

        sName = "ForcedEvolution";
        sDisplayName = "Forced Evolution";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 1, 0 });

        nCooldownInduced = 6;
        nFatigue = 1;

        lstClauses = new List<Clause>() {
            new Clause1(this),
            new Clause2(this)
        };
    }

    class Clause1 : ClauseChr {

        public int nLifeLoss = 5;

        public Clause1(Skill _skill) : base(_skill) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrRanged(this), //Base Tag always goes first
                new ClauseTagChrSelf(this)
            });

        }

        public override string GetDescription() {

            return string.Format("Lose {0} life.", nLifeLoss);
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContSkillEngine.PushSingleExecutable(new ExecLoseLife(skill.chrOwner, chrSelected, nLifeLoss) {
                sLabel = "It's going berserk"
            });

        }

    };

    class Clause2 : ClauseChr {

        public SoulEvolved soulToCopy;

        public Clause2(Skill _skill) : base(_skill) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrRanged(this), //Base Tag always goes first
                new ClauseTagChrSelf(this)
            });

            soulToCopy = new SoulEvolved(skill.chrOwner, null, skill);
        }

        public override string GetDescription() {

            return string.Format("Gain {0} POWER.", soulToCopy.nPowerBuff);
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(skill.chrOwner, chrSelected, new SoulEvolved(soulToCopy, chrSelected)) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("PitBeast/sndForcedEvolution", 4.667f) },
                sLabel = "It's evolving"
            });

        }

    };
}
