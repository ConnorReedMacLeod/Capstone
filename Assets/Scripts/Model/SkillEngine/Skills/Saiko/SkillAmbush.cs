using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAmbush : Skill {

    public SoulChannelAmbush soulChannelBehaviour;

    public SkillAmbush(Chr _chrOwner) : base(_chrOwner, 0) {//Set the dominant clause

        sName = "Ambush";
        sDisplayName = "Ambush";

        //Create an instance of the soulChannel effect
        soulChannelBehaviour = new SoulChannelAmbush(this);

        //Pass a reference into the channel-type so that it can copy our behaviour for channeling
        type = new TypeChannel(this, 4, soulChannelBehaviour);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 1 });

        nCooldownInduced = 3;
        nFatigue = 1;

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }


    class Clause1 : ClauseChr {

        public Clause1(Skill _skill) : base(_skill) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrRanged(this), //Base Tag always goes first
            });

        }

        public override string GetDescription() {

            return string.Format("While channeling, after the chosen character uses a skill, deal {0} damage to them",
                ((SkillAmbush)skill).soulChannelBehaviour.nBaseDamage);
        }

        public override void ClauseEffect(Chr chrSelected) {

            //Since this is a channel, we only have to include effects here that would happen upon
            // channel completion.  
            // For this particular skill, there's nothing

        }

    };

}