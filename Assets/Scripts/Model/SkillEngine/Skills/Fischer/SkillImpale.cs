using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillImpale : Skill {

    public Damage dmg;
    public int nBaseDamage;

    public SkillImpale(Chr _chrOwner) : base(_chrOwner, 0) {//set the dominant clause to 0

        sName = "Impale";
        sDisplayName = "Impale";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 1, 0, 0, 0, 0 });

        nCooldownInduced = 6;
        nFatigue = 2;

        nBaseDamage = 20;
        //Create a base Damage object that this skill will apply
        dmg = new Damage(this.chrOwner, null, nBaseDamage);

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : ClauseChr {

        Damage dmg;
        public int nBaseDamage = 20;
        public SoulImpaled soulToCopy;

        public Clause1(Skill _skill) : base(_skill) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrMelee(this), //Base Tag always goes first
                new ClauseTagChrEnemy(this)
            });


            dmg = new Damage(skill.chrOwner, null, nBaseDamage);
            soulToCopy = new SoulImpaled(skill.chrOwner, null, skill);
        }

        public override string GetDescription() {

            return string.Format("Deal {0} damage to the enemy Vanguard and reduce their max health by {1}.", dmg.Get(), soulToCopy.nMaxLifeReduction);
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(skill.chrOwner, chrSelected, new SoulImpaled(soulToCopy, chrSelected)));

            ContSkillEngine.PushSingleExecutable(new ExecDealDamage(skill.chrOwner, chrSelected, dmg) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Fischer/sndImpale", 1.833f) },
                sLabel = "Get Kakyoin'd"
            });


        }

    };

}
