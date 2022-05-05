using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHydrasRegen : Skill {

    public SoulChannelHydrasRegen soulChannelBehaviour;

    public SkillHydrasRegen(Chr _chrOwner) : base(_chrOwner) {

        sName = "HydrasRegen";
        sDisplayName = "Hydra's Regeneration";

        //Create an instance of the soulChannel effect
        soulChannelBehaviour = new SoulChannelHydrasRegen(this);

        //Pass a reference into the channel-type so that it can copy our behaviour for channeling
        typeUsage = new TypeUsageChannel(this, 4, soulChannelBehaviour);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 0, 0, 1, 0));

        nCooldownInduced = 6;
        nFatigue = 1;

        InitTargets();

        lstSkillClauses = new List<ClauseSkillSelection>() {
            new Clause1(this)
        };
    }

    public override void InitTargets() {
        TarMana.AddTarget(this, manaCost);
    }

    class Clause1 : ClauseSkillSelection {

        public Clause1(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {

            return string.Format("Channel ({0}).\n" +
                "While channeling, at the end of turn heal {1}.", ((TypeUsageChannel)((SkillHydrasRegen)skill).typeUsage).nStartChannelTime,
                ((SkillHydrasRegen)skill).soulChannelBehaviour.nBaseHealing);
        }

        public override void ClauseEffect(InputSkillSelection selections) {

            //Since this is a channel, we only have to include effects here that would happen upon
            // channel completion.  
            // For this particular skill, there's nothing

        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.HYDRASREGEN;
    }

}