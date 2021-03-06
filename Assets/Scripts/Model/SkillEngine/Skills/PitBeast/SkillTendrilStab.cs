﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTendrilStab : Skill {

    public SkillTendrilStab(Chr _chrOwner) : base(_chrOwner, 0) {

        sName = "TendrilStab";
        sDisplayName = "Tendril Stab";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 1, 0, 0, 0, 0 });

        nCooldownInduced = 6;
        nFatigue = 3;

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }


    class Clause1 : ClauseChr {

        Damage dmg;
        public int nBaseDamage = 25;

        public Clause1(Skill _skill) : base(_skill) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrMelee(this), //Base Tag always goes first
                new ClauseTagChrEnemy(this)
            });

            dmg = new Damage(skill.chrSource, null, nBaseDamage, true);
        }

        public override string GetDescription() {

            return string.Format("Deal {0} [PIERCING] damage to the enemy Vanguard.", dmg.Get());
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContSkillEngine.PushSingleExecutable(new ExecDealDamage(skill.chrSource, chrSelected, dmg) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("PitBeast/sndTendrilStab", 3.067f) },
                sLabel = "Stab, stab, stab"
            });

        }

    };

}
