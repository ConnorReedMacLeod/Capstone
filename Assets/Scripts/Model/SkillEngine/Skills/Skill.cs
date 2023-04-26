using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill {

    public string sName;
    public string sDisplayName;
    public TypeUsage typeUsage;

    public SkillSlot skillslot;

    public int nCooldownInduced;
    public int nFatigue;

    public Chr chrOwner;

    public bool bCharges;
    public int nCharges;
    public int nCurCharges;

    public ManaCost manaCost;

    public Property<bool> pbHidden;

    public List<Target> lstTargets;

    public List<ClauseSkillSelection> lstSkillClauses = new List<ClauseSkillSelection>();
    public List<ClauseSkill> lstClausesOnEquip = new List<ClauseSkill>();
    public List<ClauseSkill> lstClausesOnUnequip = new List<ClauseSkill>();

    public Subject subSkillChange = new Subject();

    public Skill(Chr _chrOwner) {
        chrOwner = _chrOwner;
        lstTargets = new List<Target>();
        pbHidden = new Property<bool>(true);
    }

    public abstract void InitTargets();
    public abstract SkillType.SKILLTYPE GetSkillType();

    //Changes the cost of this skill, and returns the node that is modifying that cost (so you can remove it later)
    public LinkedListNode<Property<Mana>.Modifier> ChangeCost(Property<Mana>.Modifier modifier) {

        LinkedListNode<Property<Mana>.Modifier> nodeModifier = manaCost.pManaCost.AddModifier(modifier);

        //Let others know that the cost has changed
        subSkillChange.NotifyObs();

        return nodeModifier;
    }

    //What should happen when this skill is added to the list of skills
    public virtual void OnEquip() {

        ContSkillEngine.PushClauses(lstClausesOnEquip);

    }

    //What should happen when this skill is removed from the list of skills
    public virtual void OnUnequip() {

        ContSkillEngine.PushClauses(lstClausesOnUnequip);

    }

    //TODO - don't make new instances of these, just store one modifiable copy in the skill and 
    //       push a reference to it onto the stack instead
    public void PaySkillPoints() {

        ContSkillEngine.PushSingleClause(new ClausePaySkillPoints(this));

    }

    public void PayManaCost() {

        ContSkillEngine.PushSingleClause(new ClausePayMana(this));

    }

    public void PayCooldown() {

        ContSkillEngine.PushSingleClause(new ClausePayCooldown(this));
    }

    public void PayFatigue() {

        ContSkillEngine.PushSingleClause(new ClausePayFatigue(this));

    }

    public void PushStartingMarker() {

        ContSkillEngine.PushSingleClause(new ClauseStartSkill(this));

    }

    public void PushEndingMarker() {

        ContSkillEngine.PushSingleClause(new ClauseEndSkill(this));

    }

    //Use the selected skill with the supplied targets
    public void UseSkill() {

        if(NetworkMatchReceiver.Get().IsCurMatchInputReady() == false) {
            Debug.LogError("Tried to use skill, but we don't have the current input ready yet (input " + NetworkMatchReceiver.Get().indexCurMatchInput);
        }

        if(CanSelect((InputSkillSelection)NetworkMatchReceiver.Get().GetCurMatchInput()) == false) {
            Debug.LogError("Tried to use skill, but the master-provided selections were invalid! : " + NetworkMatchReceiver.Get().GetCurMatchInput().ToString());
        }

        // IMPORTANT - since we're pushing these effects onto the stack, we'll want to 
        //             push them in reverse order so that we will evaluate the most recently pushed effect first



        //Finally, add an ending marker after the skill is executed
        PushEndingMarker();

        //Let the type of this skill dictate the behaviour and push all relevant effects onto the stack
        typeUsage.UseSkill();

        //Then, add a starting marker before the skills' effects
        PushStartingMarker();

        //Fourth, pay the cooldown
        PayCooldown();

        //Third, pay the fatigue
        PayFatigue();

        //Second pay the mana cost for the skill
        PayManaCost();

        //First pay the skill points
        PaySkillPoints();

        //Now that all the effects of the skill are pushed onto the stack, we can make this skill publicly visible
        pbHidden.SetBase(false);
    }


    public void Execute() {

        //Push a reference to each of our clauses onto the stack so they may be evaluated later
        ContSkillEngine.PushClauses(lstSkillClauses);
    }

    //Determine if the skill could be used targetting the passed selections 
    public bool CanSelect(InputSkillSelection selections) {

        //First, check if we're at least alive
        if(chrOwner.bDead == true) {
            return false;
        }

        //Check that we're in a readiness state (with enough usable skills left)
        if(chrOwner.curStateReadiness.CanSelectSkill(this) == false) {
            //Debug.Log("Not in a state where we can use this skill");
            return false;
        }

        //Check that the skill isn't on cooldown
        if(skillslot.IsOffCooldown() == false) {
            //Debug.Log ("Skill on cd");
            return false;
        }

        if(selections.HasLegallyFilledTargets() == false) {
            Debug.Log("This selection has an invalid choice");
            return false;
        }

        return true;
    }

    //Determine if the skill should continue as a channel.  If the character dies, or has no valid targets for completing
    //  the channel targetting, then we can cancel it
    public bool CanCompleteAsChannel() {

        //First, check if we're at least alive
        if(chrOwner.bDead == true) {
            return false;
        }

        //Note, we need to be in a channeling state, but a channel will automatically stop if we ever leave the channeling state anyway
        Debug.Assert(chrOwner.curStateReadiness.Type() == StateReadiness.TYPE.CHANNELING);

        if(ExtraCanCompleteAsChannelChecks() == false) {
            Debug.Log("Cannot complete channel due to skill-specific checks");
            return false;
        }

        Debug.Log("WARNING - remember to consider what global checks need to be done to see if a channel should be cancelled");

        return true;
    }

    //Overload as needed for channels to listen for triggers that may force the channel to automatically end early due to invalid targets
    public virtual List<Subject> GetPotentialCancelTriggers() {
        //By default, we'll return null - if any channels uses this base-method it'll likely crash alerting you to the fact that you
        //  really should have some triggers that your channel should cancel under.  If there's really nothing, just override this to produce an empty list
        return null;
    }

    //Overload as needed for skill-specific checks for if that channel is in a state where it could be cancelled
    public virtual bool ExtraCanCompleteAsChannelChecks() {
        //By default, no extra checks are performed
        return true;
    }

    public static bool IsEnemy(Chr owner, Chr tar) {
        return owner.plyrOwner != tar.plyrOwner;
    }

    public static bool IsFriendly(Chr owner, Chr tar) {
        return owner.plyrOwner == tar.plyrOwner;
    }

    public static bool IsAnyCharacter(Chr owner, Chr tar) {
        return true;
    }

    public bool IsStandardSkill() {
        return skillslot.iSlot < Chr.nEquippedCharacterSkills;
    }

    public bool IsGenericSkill() {
        return skillslot.iSlot >= Chr.nEquippedCharacterSkills;
    }

    class ClausePayMana : ClauseSkill {

        public ClausePayMana(Skill _skill) : base(_skill) {
        }

        public override string GetDescription() {
            return string.Format("Paying mana for skill");
        }

        public override void Execute() {

            //TODO:: Think if this is an acceptable solution, since it seems potentially narrow.  It also assumes that the mana you reserved
            //       exactly corresponds to the mana you NEED to pay.  I guess this helps with any optional costs that might get played, but I
            //       worry that it'd be possible to un-reserve mana somehow to avoid actually paying the requisite amounts for the skill's costs

            //We'll spend all the mana that we've reserved as part of the mana-paying process of targetting and then clear out reserved mana
            ContSkillEngine.PushSingleExecutable(new ExecChangeMana(skill.chrOwner, skill.chrOwner.plyrOwner,
                Mana.GetNegatedMana(skill.chrOwner.plyrOwner.manapool.manaReservedToPay)));

            skill.chrOwner.plyrOwner.manapool.ResetReservedMana();
        }

    };

    class ClausePayCooldown : ClauseSkill {

        public ClausePayCooldown(Skill _skill) : base(_skill) {
        }

        public override string GetDescription() {
            return string.Format("Paying cooldown for skill");
        }

        public override void Execute() {

            ContSkillEngine.PushSingleExecutable(new ExecChangeCooldown(skill.chrOwner, skill.skillslot, skill.nCooldownInduced));

        }

    };

    class ClausePayFatigue : ClauseSkill {

        public ClausePayFatigue(Skill _skill) : base(_skill) {
        }

        public override string GetDescription() {
            return string.Format("Paying fatigue for skill");
        }

        public override void Execute() {

            ContSkillEngine.PushSingleExecutable(new ExecChangeFatigue(skill.chrOwner, skill.chrOwner, skill.nFatigue));

        }

    };

    class ClausePaySkillPoints : ClauseSkill {

        public ClausePaySkillPoints(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {
            return string.Format("Paying skill points");
        }

        public override void Execute() {
            ContSkillEngine.PushSingleExecutable(new ExecPaySkillPoints(skill.chrOwner, skill.chrOwner, skill));
        }
    };

    class ClauseStartSkill : ClauseSkill {

        public ClauseStartSkill(Skill _skill) : base(_skill) {
        }

        public override string GetDescription() {
            return string.Format("Starting marker for this skill");
        }

        public override void Execute() {

            ContSkillEngine.PushSingleExecutable(new ExecStartSkill(skill.chrOwner, skill));

        }

    };

    class ClauseEndSkill : ClauseSkill {

        public ClauseEndSkill(Skill _skill) : base(_skill) {
        }

        public override string GetDescription() {
            return string.Format("Ending marker for this skill");
        }

        public override void Execute() {

            ContSkillEngine.PushSingleExecutable(new ExecEndSkill(skill.chrOwner, skill));

        }

    };


    public override string ToString() {
        return sDisplayName;
    }
}
