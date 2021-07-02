using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHiss : Skill {


    public SkillHiss(Chr _chrOwner) : base(_chrOwner, 0) {//Set the dominant clause

        sName = "Hiss";
        sDisplayName = "Hiss";

        type = new TypeCantrip(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 1 });

        nCooldownInduced = 10;
        nFatigue = 1;

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : ClauseChr {

        public SoulSpooked soulToCopy;

        public Clause1(Skill _skill) : base(_skill) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrSweeping(this), //Base Tag always goes first
                new ClauseTagChrEnemy(this)
            });

            soulToCopy = new SoulSpooked(skill.chrSource, null, skill);
        }

        public override string GetDescription() {

            return string.Format("All enemies lose {0} POWER for {1} turns.", soulToCopy.nPowerDebuff, soulToCopy.pnMaxDuration.Get());
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(skill.chrSource, chrSelected, new SoulSpooked(soulToCopy, chrSelected)) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Sophidia/sndHiss1", 2f),
                                                     new SoundEffect("Sophidia/sndHiss2", 2f),
                                                     new SoundEffect("Sophidia/sndHiss3", 2f)},
                sLabel = "Ah, so spook!"
            });

        }

    };

}