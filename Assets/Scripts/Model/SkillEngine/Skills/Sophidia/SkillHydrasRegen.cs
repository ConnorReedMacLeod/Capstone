﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHydrasRegen : Skill {

    public SoulChannelHydrasRegen soulChannelBehaviour;

    public SkillHydrasRegen(Chr _chrOwner) : base(_chrOwner, 0) {//Set the dominant clause

        sName = "HydrasRegen";
        sDisplayName = "Hydra's Regeneration";

        //Create an instance of the soulChannel effect
        soulChannelBehaviour = new SoulChannelHydrasRegen(this);

        //Pass a reference into the channel-type so that it can copy our behaviour for channeling
        type = new TypeChannel(this, 4, soulChannelBehaviour);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 1, 0 });

        nCooldownInduced = 6;
        nFatigue = 1;

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : ClauseChr {

        public Clause1(Skill _skill) : base(_skill) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrRanged(this), //Base Tag always goes first
                new ClauseTagChrSelf(this)
            });

        }

        public override string GetDescription() {

            return string.Format("Channel ({0}).\n" +
                "While channeling, at the end of turn heal {1}.", ((TypeChannel)((SkillHydrasRegen)skill).type).nStartChannelTime,
                ((SkillHydrasRegen)skill).soulChannelBehaviour.nBaseHealing);
        }

        public override void ClauseEffect(Chr chrSelected) {

            //Since this is a channel, we only have to include effects here that would happen upon
            // channel completion.  
            // For this particular skill, there's nothing

        }

    };

}