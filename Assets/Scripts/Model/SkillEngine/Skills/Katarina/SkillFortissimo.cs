using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillFortissimo : Skill {

    public SkillFortissimo(Chr _chrOwner) : base(_chrOwner, 0) {//Set the dominant clause

        sName = "Fortissimo";
        sDisplayName = "Fortissimo";

        type = new TypeCantrip(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 0 });

        nCooldownInduced = 8;
        nFatigue = 0;

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }


    class Clause1 : ClauseChr {

        public SoulFortissimo soulToCopy;

        public Clause1(Skill _skill) : base(_skill) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrRanged(this), //Base Tag always goes first
                new ClauseTagChrSelf(this)
            });

            soulToCopy = new SoulFortissimo(skill.chrSource, null, skill);
        }

        public override string GetDescription() {

            return string.Format("Gain {0} POWER and {1} DEFENSE for {2} turns.", soulToCopy.nPowerBuff, soulToCopy.nDefenseBuff, soulToCopy.pnMaxDuration.Get());
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(skill.chrSource, chrSelected, new SoulFortissimo(soulToCopy, chrSelected)) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Katarina/sndFortissimo", 6.2f) },
                sLabel = "Let's do it louder this time"
            });

        }

    };
}
