using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill { //This should probably be made abstract

    public string sName;
    public string sDisplayName;
    public TypeSkill type;

    public SkillSlot skillslot;

    public int nCooldownInduced;
    public int nFatigue;

    public Chr chrOwner;

    public bool bCharges;
    public int nCharges;
    public int nCurCharges;

    public ManaCost manaCost;

    public List<Target> lstTargets;

    public List<Clause> lstClauses = new List<Clause>();
    public List<Clause> lstClausesOnEquip = new List<Clause>();
    public List<Clause> lstClausesOnUnequip = new List<Clause>();

    public Subject subSkillChange = new Subject();

    public Skill(Chr _chrOwner) {
        chrOwner = _chrOwner;
        lstTargets = new List<Target>();
    }

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

    public bool CanPayMana() {
        //Check if you have enough mana
        if(chrOwner.plyrOwner.mana.CanPayManaCost(this.manaCost) == false) {
            Debug.Log("Not enough mana");
            return false;
        }
        return true;
    }

    //Use the selected skill with the supplied targets
    public void UseSkill() {

        if(CanSelect(ContSkillSelection.Get().selectionsFromMaster) == false) {
            Debug.LogError("Tried to use skill, but the master-provided selections were invalid! : " + ContSkillSelection.Get().selectionsFromMaster.ToString());
        }

        // IMPORTANT - since we're pushing these effects onto the stack, we'll want to 
        //             push them in reverse order so that we will evaluate the most recently pushed effect first



        //Finally, add an ending marker after the skill is executed
        PushEndingMarker();

        //Let the type of this skill dictate the behaviour and push all relevant effects onto the stack
        type.UseSkill();

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
    }


    public void Execute() {

        //Push a reference to each of our clauses onto the stack so they may be evaluated later
        ContSkillEngine.PushClauses(lstClauses);
    }

    //Determine if the skill could be used targetting the passed selections 
    public virtual bool CanSelect(Selections selections) {// Maybe this doesn't need to be virtual

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

    //Determine if the skill can be executed with the given selection parameters - this is more allowable
    //  since we just want to ensure a prepped skill will not fizzle if it can at least do something
    public bool CanExecute(Selections selections) {

        //First, check if we're at least alive
        if(chrOwner.bDead == true) {
            return false;
        }

        //Check that we're in a readiness state (with enough usable skills left)
        if(chrOwner.curStateReadiness.CanSelectSkill(this) == false) {
            //Debug.Log("Not in a state where we can use this skill");
            return false;
        }

        //Finally, check if enough of the targets are valid to let the skill execute (if some become invalid,
        // this may be okay, but it depends on the ability)
        if(selections.IsGoodEnoughToExecute() == false) {
            Debug.Log("Skill cannot execute due to some number of invalid selections");
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

        Debug.Log("WARNING - remember to consider what needs to be checked to see if a channel should be cancelled");

        //This used to just check if the dominant clause could find a valid target, but we don't have that anymore

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
        return skillslot.iSlot < Chr.nStandardCharacterSkills;
    }

    public bool IsGenericSkill() {
        return skillslot.iSlot >= Chr.nStandardCharacterSkills;
    }

    class ClausePayMana : Clause {

        public ClausePayMana(Skill _skill) : base(_skill) {
        }

        public override string GetDescription() {
            return string.Format("Paying mana for skill");
        }

        public override void ClauseEffect(Selections selections) {

            ContSkillEngine.PushSingleExecutable(new ExecChangeMana(skill.chrOwner, skill.chrOwner.plyrOwner, Mana.GetNegatedMana(skill.manaCost.pManaCost.Get())));

        }

    };

    class ClausePayCooldown : Clause {

        public ClausePayCooldown(Skill _skill) : base(_skill) {
        }

        public override string GetDescription() {
            return string.Format("Paying cooldown for skill");
        }

        public override void ClauseEffect(Selections selections) {

            ContSkillEngine.PushSingleExecutable(new ExecChangeCooldown(skill.chrOwner, skill, skill.nCooldownInduced));

        }

    };

    class ClausePayFatigue : Clause {

        public ClausePayFatigue(Skill _skill) : base(_skill) {
        }

        public override string GetDescription() {
            return string.Format("Paying fatigue for skill");
        }

        public override void ClauseEffect(Selections selection) {

            ContSkillEngine.PushSingleExecutable(new ExecChangeFatigue(skill.chrOwner, skill.chrOwner, skill.nFatigue, false));

        }

    };

    class ClausePaySkillPoints : Clause {

        public ClausePaySkillPoints(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {
            return string.Format("Paying skill points");
        }

        public override void ClauseEffect(Selections selections) {
            ContSkillEngine.PushSingleExecutable(new ExecPaySkillPoints(skill.chrOwner, skill.chrOwner, skill));
        }
    };

    class ClauseStartSkill : Clause {

        public ClauseStartSkill(Skill _skill) : base(_skill) {
        }

        public override string GetDescription() {
            return string.Format("Starting marker for this skill");
        }

        public override void ClauseEffect(Selections selections) {

            ContSkillEngine.PushSingleExecutable(new ExecStartSkill(skill.chrOwner, skill));

        }

    };

    class ClauseEndSkill : Clause {

        public ClauseEndSkill(Skill _skill) : base(_skill) {
        }

        public override string GetDescription() {
            return string.Format("Ending marker for this skill");
        }

        public override void ClauseEffect(Selections selections) {

            ContSkillEngine.PushSingleExecutable(new ExecEndSkill(skill.chrOwner, skill));

        }

    };

}
