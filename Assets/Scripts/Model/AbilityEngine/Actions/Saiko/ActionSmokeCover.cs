﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSmokeCover : Action {

    public ActionSmokeCover(Chr _chrOwner) : base(_chrOwner, 0) {//Set the dominant clause

        sName = "SmokeCover";
        sDisplayName = "Smoke Cover";

        type = new TypeCantrip(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 1, 0, 0, 0 });

        nCd = 10;
        nFatigue = 2;

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : ClauseChr {

        public SoulCloudCushion soulToCopy;

        public Clause1(Action _act) : base(_act) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrRanged(this), //Base Tag always goes first
                new ClauseTagChrSelf(this)
            });

            soulToCopy = new SoulCloudCushion(action.chrSource, null, action);
        }

        public override string GetDescription() {

            return string.Format("Gain SHROUDED (4)\n" +
                "[SHROUDED]: This character is immune to damage.  If this character becomes the Vanguard, dispel this.", soulToCopy.nDefenseBuff, soulToCopy.pnMaxDuration.Get());
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContAbilityEngine.PushSingleExecutable(new ExecApplySoul(action.chrSource, chrSelected, new SoulCloudCushion(soulToCopy, chrSelected)) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Saiko/sndSmokeCover", 4.3f) },
                sLabel = "Disappearing into the shadows..."
            });

        }

    };

}
