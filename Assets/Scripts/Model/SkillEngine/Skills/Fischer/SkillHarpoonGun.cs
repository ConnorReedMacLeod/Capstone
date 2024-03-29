﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHarpoonGun : Skill {

    public SkillHarpoonGun(Chr _chrOwner) : base(_chrOwner) {

        sName = "HarpoonGun";
        sDisplayName = "Harpoon Gun";

        //We don't have any specific effect to take place while channeling, so just leave the
        // soulChannel effect null and let it copy our execution's effect for what it does when the channel completes
        typeUsage = new TypeUsageChannel(this, 2, null);

        //Physical, Mental, Energy, Blood, Effort
        manaCost = new ManaCost(new Mana(0, 0, 0, 0, 2));

        nCooldownInduced = 5;
        nFatigue = 2;

        InitTargets();

        lstSkillClauses = new List<ClauseSkillSelection>() {
            new Clause1(this),
            new Clause2(this)
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


        InputSkillSelection storedSelections = ((TypeUsageChannel)typeUsage).GetUsedSelections();
        List<object> lstStoredSelections = storedSelections.lstSelections;

        //Ensure the Chr we're targetting is generally still a legal target (i.e., not dead or on the bench - no further extensions are needed)
        if (((TarChr)lstTargets[1]).DefaultCanCompleteAsChannelTarget((Chr)lstStoredSelections[1], storedSelections) == false) {
            Debug.LogFormat("Cancelling channel since {0} is no longer targetting a legal character ({1})", this, (Chr)lstStoredSelections[1]);
            return false;
        }

        return true;
    }


    class Clause1 : ClauseSkillSelection {

        Damage dmg;
        public int nBaseDamage = 30;

        public Clause1(Skill _skill) : base(_skill) {
            dmg = new Damage(skill.chrOwner, null, nBaseDamage);
        }

        public override string GetDescription() {

            return string.Format("After channeling, deal {0} damage to the chosen enemy.", dmg.Get());
        }

        public override void ClauseEffect(InputSkillSelection selections) {

            Chr chrSelected = (Chr)selections.lstSelections[1];

            ContSkillEngine.PushSingleExecutable(new ExecDealDamage(skill.chrOwner, chrSelected, dmg) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Fischer/sndHarpoonGun", 2.067f) },
                sLabel = "Behold, the power of my stand, Beach Boy!"
            });

        }

    };

    class Clause2 : ClauseSkillSelection {

        public Clause2(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {

            return string.Format("That enemy Switches to the position in front of them");
        }

        public override void ClauseEffect(InputSkillSelection selections) {

            Chr chrSelected = (Chr)selections.lstSelections[1];

            ContSkillEngine.PushSingleExecutable(new ExecSwitchChar(skill.chrOwner, chrSelected, (chrTarget) => ContPositions.Get().GetInFrontPosition(chrTarget.position)) {
                sLabel = "Hey, I caught one!"
            });

        }

    };

    public override SkillType.SKILLTYPE GetSkillType() {
        return SkillType.SKILLTYPE.HARPOONGUN;
    }

}