using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAmbush : Skill {

    public SoulChannelAmbush soulChannelBehaviour;

    public SkillAmbush(Chr _chrOwner) : base(_chrOwner) {

        sName = "Ambush";
        sDisplayName = "Ambush";

        //Create an instance of the soulChannel effect
        soulChannelBehaviour = new SoulChannelAmbush(this);

        //Pass a reference into the channel-type so that it can copy our behaviour for channeling
        typeUsage = new TypeUsageChannel(this, 4, soulChannelBehaviour);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 0, 0, 0, 1));

        nCooldownInduced = 3;
        nFatigue = 1;

        InitTargets();

        lstSkillClauses = new List<ClauseSkillSelection>() {
            new Clause1(this)
        };
    }

    public override void InitTargets() {
        TarMana.AddTarget(this, manaCost);
        TarChr.AddTarget(this, Target.AND(TarChr.IsInPlay(), TarChr.IsDiffTeam(chrOwner)));
    }


    //Channels should usually have situations where they should automatically cancel if their targetting becomes invalid
    public override List<Subject> GetPotentialCancelTriggers() {

        //Fetch a reference to the selections that have been made for this skill
        List<object> lstStoredSelections = ((TypeUsageChannel)typeUsage).GetUsedSelections().lstSelections;

        List<Subject> lstTriggers = new List<Subject>();

        //Add the recommended default triggers for the Chr we're targetting
        ((TarChr)lstTargets[1]).AddDefaultTriggersToCompleteAsChannel(lstTriggers, (Chr)lstStoredSelections[1]);

        return lstTriggers;
    }

    public override bool ExtraCanCompleteAsChannelChecks() {

        List<object> lstStoredSelections = ((TypeUsageChannel)typeUsage).GetUsedSelections().lstSelections;

        //Ensure the Chr we're targetting is generally still a legal target (i.e., not dead or on the bench - no further extensions are needed)
        if (((TarChr)lstTargets[1]).DefaultCanCompleteAsChannelTarget((Chr)lstStoredSelections[1]) == false) {
            Debug.LogFormat("Cancelling channel since {0} is no longer targetting a legal character ({1})", this, (Chr)lstStoredSelections[1]);
            return false;
        }

        return true;
    }


    class Clause1 : ClauseSkillSelection {

        public Clause1(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {

            return string.Format("While channeling, after the chosen character uses a skill, deal {0} damage to them",
                ((SkillAmbush)skill).soulChannelBehaviour.nBaseDamage);
        }

        public override void ClauseEffect(InputSkillSelection selections) {

            //Since this is a channel, we only have to include effects here that would happen upon
            // channel completion.  
            // For this particular skill, there's nothing

        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.AMBUSH;
    }

}