using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTwinSnakes : Skill {

    public SkillTwinSnakes(Chr _chrOwner) : base(_chrOwner, 0) {

        sName = "TwinSnakes";
        sDisplayName = "Twin Snakes";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 1, 1 });

        nCooldownInduced = 8;
        nFatigue = 4;

        lstClauses = new List<Clause>() {
            new Clause1(this),
            new Clause1(this)
        };
    }

    class Clause1 : ClauseChr {

        Damage dmg;
        public int nBaseDamage = 20;
        public int nLifeloss = 5;

        public Clause1(Skill _skill) : base(_skill) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrRanged(this) //Base Tag always goes first
            });


            dmg = new Damage(skill.chrSource, null, nBaseDamage);
        }

        public override string GetDescription() {

            return string.Format("Deal {0} damage to the chosen character.  Lose {1} health", dmg.Get(), nLifeloss);
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContSkillEngine.PushSingleExecutable(new ExecLoseLife(skill.chrSource, skill.chrSource, nLifeloss) {
                sLabel = "Owie"
            });

            ContSkillEngine.PushSingleExecutable(new ExecDealDamage(skill.chrSource, chrSelected, dmg) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Sophidia/sndTwinSnakes", 2f) },
                sLabel = "Snakey, no!"
            });

        }

    };

}