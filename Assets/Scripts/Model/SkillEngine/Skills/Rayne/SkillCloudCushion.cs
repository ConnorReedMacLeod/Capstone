using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCloudCushion : Skill {

    public SkillCloudCushion(Chr _chrOwner) : base(_chrOwner, 0) {//set the dominant clause

        sName = "CloudCushion";
        sDisplayName = "Cloud Cushion";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 1, 0, 0 });

        nCooldownInduced = 7;
        nFatigue = 1;

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : ClauseChr {

        public SoulCloudCushion soulToCopy;

        public Clause1(Skill _skill) : base(_skill) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrRanged(this), //Base Tag always goes first
                new ClauseTagChrAlly(this)
            });

            soulToCopy = new SoulCloudCushion(skill.chrOwner, null, skill);
        }

        public override string GetDescription() {

            return string.Format("Target ally gains {0} DEFENSE for {1} turns.", soulToCopy.nDefenseBuff, soulToCopy.pnMaxDuration.Get());
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(skill.chrOwner, chrSelected, new SoulCloudCushion(soulToCopy, chrSelected)) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Rayne/sndCloudCushion", 3.467f) },
                sLabel = "Ooh, so soft"
            });

        }

    };

}