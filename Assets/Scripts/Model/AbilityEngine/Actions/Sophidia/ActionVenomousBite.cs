﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionVenomousBite : Action {

    public ActionVenomousBite(Chr _chrOwner) : base(_chrOwner, 0) {// Set the dominant clause

        sName = "VenomousBite";
        sDisplayName = "Venomous Bite";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 1, 0 });

        nCd = 8;
        nFatigue = 3;

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : ClauseChr {

        Damage dmg;
        public int nBaseDamage = 5;
        public SoulEnvenomed soulToCopy;

        public Clause1(Action _act) : base(_act) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrMelee(this), //Base Tag always goes first
                new ClauseTagChrEnemy(this)
            });

            dmg = new Damage(action.chrSource, null, nBaseDamage);
            soulToCopy = new SoulEnvenomed(action.chrSource, null, action);
        }

        public override string GetDescription() {

            return string.Format("Deal {0} damage and apply ENVENOMED ({1}) to the enemy Vanguard.\n" +
                "[ENVENOMED]: At the end of turn, lose {2} health.  Whenever this character takes damage, +1 duration.", dmg.Get(), soulToCopy.pnMaxDuration.Get(),
                soulToCopy.nLifeLoss);
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContAbilityEngine.PushSingleExecutable(new ExecApplySoul(action.chrSource, chrSelected, new SoulEnvenomed(soulToCopy, chrSelected)) {
                sLabel = "Applying poison"
            });

            ContAbilityEngine.PushSingleExecutable(new ExecDealDamage(action.chrSource, chrSelected, dmg) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Sophidia/sndVenomousBite1", 2f),
                                                     new SoundEffect("Sophidia/sndVenomousBite2", 2f)},
                sLabel = "Nomnomnom"
            });

        }

    };
    
}
