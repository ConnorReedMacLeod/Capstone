using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSmokeCover : Skill {

    public SkillSmokeCover(Chr _chrOwner) : base(_chrOwner, 0) {//Set the dominant clause

        sName = "SmokeCover";
        sDisplayName = "Smoke Cover";

        type = new TypeCantrip(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 1, 0, 0, 0 });

        nCooldownInduced = 10;
        nFatigue = 2;

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : ClauseChr {

        public SoulSmokeCover soulToCopy;

        public Clause1(Skill _skill) : base(_skill) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrRanged(this), //Base Tag always goes first
                new ClauseTagChrSelf(this)
            });

            soulToCopy = new SoulSmokeCover(skill.chrOwner, null, skill);
        }

        public override string GetDescription() {

            return string.Format("Gain SHROUDED (4)\n" +
                "[SHROUDED]: This character is immune to damage.  If this character becomes the Vanguard, dispel this.");
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(skill.chrOwner, chrSelected, new SoulSmokeCover(soulToCopy, chrSelected)) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Saiko/sndSmokeCover", 4.3f) },
                sLabel = "Disappearing into the shadows..."
            });

        }

    };

}
