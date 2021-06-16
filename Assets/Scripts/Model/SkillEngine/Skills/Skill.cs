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

    public Chr chrSource;

    public bool bCharges;
    public int nCharges;
    public int nCurCharges;

    public Property<int[]> parCost;

    public List<Clause> lstClauses = new List<Clause>();
    public List<Clause> lstClausesOnEquip = new List<Clause>();
    public List<Clause> lstClausesOnUnequip = new List<Clause>();

    public int iDominantClause;

    public Subject subSkillChange = new Subject();

    public Skill(Chr _chrOwner, int _iDominantClause) {
        chrSource = _chrOwner;
        iDominantClause = _iDominantClause;

    }

    public Clause.TargetType GetTargetType() {
        return GetDominantClause().targetType;
    }

    public Clause GetDominantClause() {
        return lstClauses[iDominantClause];
    }

    //Changes the cost of this skill, and returns the node that is modifying that cost (so you can remove it later)
    public LinkedListNode<Property<int[]>.Modifier> ChangeCost(Property<int[]>.Modifier modifier) {

        LinkedListNode<Property<int[]>.Modifier> nodeModifier = parCost.AddModifier(modifier);

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
        if(chrSource.plyrOwner.mana.HasMana(parCost.Get()) == false) {
            Debug.Log("Not enough mana");
            return false;
        }
        return true;
    }

    //Use the selected skill with the supplied targets
    public void UseSkill(SelectionSerializer.SelectionInfo infoSelection) {

        if(CanPayMana() == false) {
            Debug.LogError("Tried to use skill, but didn't have enough mana");
        }

        if(CanSelect(infoSelection) == false) {
            Debug.LogError("Tried to use skill, but it's not a valid selection");
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

    //Determine if the skill could be used targetting the passed indices (Note: doesn't include mana check)
    public virtual bool CanSelect(SelectionSerializer.SelectionInfo selectionInfo) {// Maybe this doesn't need to be virtual

        //First, check if we're at least alive
        if(chrSource.bDead == true) {
            return false;
        }

        //Check that we're in a readiness state (with enough usable skills left)
        if(chrSource.curStateReadiness.CanSelectSkill(this) == false) {
            //Debug.Log("Not in a state where we can use this skill");
            return false;
        }

        //Check that the skill isn't on cooldown
        if(skillslot.IsOffCooldown() == false) {
            //Debug.Log ("Skill on cd");
            return false;
        }

        if(lstClauses[iDominantClause].IsSelectable(selectionInfo) == false) {
            Debug.Log("This selection would make the dominant clause invalid");
            return false;
        }

        return true;
    }

    //Determine if the skill can be executed with the given selection parameters - this is more allowable
    //  since we just want to ensure a prepped skill will not fizzle if it can at least do something
    public bool CanExecute(SelectionSerializer.SelectionInfo selectionInfo) {

        //First, check if we're at least alive
        if(chrSource.bDead == true) {
            return false;
        }

        //Check that we're in a readiness state (with enough usable skills left)
        if(chrSource.curStateReadiness.CanSelectSkill(this) == false) {
            //Debug.Log("Not in a state where we can use this skill");
            return false;
        }

        //Finally, check if there's at least one valid target to execute on - some skill clauses
        // without targets won't make sense to execute if the dominant clause has no targets.
        //  E.g. - a vampire bite's healing clause wouldn't make sense to execute if its
        //         damage clause has no viable target
        if(lstClauses[iDominantClause].HasFinalTarget(selectionInfo) == false) {
            Debug.Log("This selection would make the dominant clause invalid");
            return false;
        }

        return true;
    }

    //Determine if the skill should continue as a channel.  If the character dies, or has no valid targets for completing
    //  the channel targetting, then we can cancel it
    public bool CanCompleteAsChannel(SelectionSerializer.SelectionInfo selectionInfo) {

        //First, check if we're at least alive
        if(chrSource.bDead == true) {
            return false;
        }

        //Finally, check if there's at least one valid target to execute on - some skill clauses
        // without targets won't make sense to execute if the dominant clause has no targets.
        //  E.g. - a vampire bite's healing clause wouldn't make sense to execute if its
        //         damage clause has no viable target
        if(lstClauses[iDominantClause].HasFinalTarget(selectionInfo) == false) {
            Debug.Log("This " + sName + " channel cannot complete since it has no valid targets");
            return false;
        }

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

    class ClausePayMana : ClauseSpecial {

        public ClausePayMana(Skill _skill) : base(_skill) {
        }

        public override string GetDescription() {
            return string.Format("Paying mana for skill");
        }

        public override void ClauseEffect() {

            ContSkillEngine.PushSingleExecutable(new ExecChangeMana(skill.chrSource, skill.chrSource.plyrOwner, Mana.ConvertToCost(skill.parCost.Get())));

        }

    };

    class ClausePayCooldown : ClauseSpecial {

        public ClausePayCooldown(Skill _skill) : base(_skill) {
        }

        public override string GetDescription() {
            return string.Format("Paying cooldown for skill");
        }

        public override void ClauseEffect() {

            ContSkillEngine.PushSingleExecutable(new ExecChangeCooldown(skill.chrSource, skill, skill.nCooldownInduced));

        }

    };

    class ClausePayFatigue : ClauseSpecial {

        public ClausePayFatigue(Skill _skill) : base(_skill) {
        }

        public override string GetDescription() {
            return string.Format("Paying fatigue for skill");
        }

        public override void ClauseEffect() {

            ContSkillEngine.PushSingleExecutable(new ExecChangeFatigue(skill.chrSource, skill.chrSource, skill.nFatigue, false));

        }

    };

    class ClausePaySkillPoints : ClauseSpecial {

        public ClausePaySkillPoints(Skill _skill) : base(_skill) {

        }

        public override string GetDescription() {
            return string.Format("Paying skill points");
        }

        public override void ClauseEffect() {
            ContSkillEngine.PushSingleExecutable(new ExecPaySkillPoints(skill.chrSource, skill.chrSource, skill));
        }
    };

    class ClauseStartSkill : ClauseSpecial {

        public ClauseStartSkill(Skill _skill) : base(_skill) {
        }

        public override string GetDescription() {
            return string.Format("Starting marker for this skill");
        }

        public override void ClauseEffect() {

            ContSkillEngine.PushSingleExecutable(new ExecStartSkill(skill.chrSource, skill));

        }

    };

    class ClauseEndSkill : ClauseSpecial {

        public ClauseEndSkill(Skill _skill) : base(_skill) {
        }

        public override string GetDescription() {
            return string.Format("Ending marker for this skill");
        }

        public override void ClauseEffect() {

            ContSkillEngine.PushSingleExecutable(new ExecEndSkill(skill.chrSource, skill));

        }

    };

}
