using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action { //This should probably be made abstract

    public string sName;
    public string sDisplayName;
    public TypeAction type;

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

    public Subject subAbilityChange = new Subject();

    public Action(Chr _chrOwner, int _iDominantClause) {
        chrSource = _chrOwner;
        iDominantClause = _iDominantClause;

    }

    public Clause.TargetType GetTargetType() {
        return GetDominantClause().targetType;
    }

    public Clause GetDominantClause() {
        return lstClauses[iDominantClause];
    }

    //Changes the cost of this action, and returns the node that is modifying that cost (so you can remove it later)
    public LinkedListNode<Property<int[]>.Modifier> ChangeCost(Property<int[]>.Modifier modifier) {

        LinkedListNode<Property<int[]>.Modifier> nodeModifier = parCost.AddModifier(modifier);

        //Let others know that the cost has changed
        subAbilityChange.NotifyObs();

        return nodeModifier;
    }

    //What should happen when this action is added to the list of abilities
    public virtual void OnEquip() {

        ContAbilityEngine.PushClauses(lstClausesOnEquip);

    }

    //What should happen when this action is remove from the list of abilities
    public virtual void OnUnequip() {

        ContAbilityEngine.PushClauses(lstClausesOnUnequip);

    }

    //TODO - don't make new instances of these, just store one modifiable copy in the action and 
    //       push a reference to it onto the stack instead
    public void PayActionPoints() {

        ContAbilityEngine.PushSingleClause(new ClausePayActionPoints(this));

    }

    public void PayManaCost() {

        ContAbilityEngine.PushSingleClause(new ClausePayMana(this));

    }

    public void PayCooldown() {

        ContAbilityEngine.PushSingleClause(new ClausePayCooldown(this));
    }

    public void PayFatigue() {

        ContAbilityEngine.PushSingleClause(new ClausePayFatigue(this));

    }

    public void PushStartingMarker() {

        ContAbilityEngine.PushSingleClause(new ClauseStartAbility(this));

    }

    public void PushEndingMarker() {

        ContAbilityEngine.PushSingleClause(new ClauseEndAbility(this));

    }

    public bool CanPayMana() {
        //Check if you have enough mana
        if(chrSource.plyrOwner.mana.HasMana(parCost.Get()) == false) {
            Debug.Log("Not enough mana");
            return false;
        }
        return true;
    }

    //Use the selected action with the supplied targets
    public void UseAction(SelectionSerializer.SelectionInfo infoSelection) {

        if(CanPayMana() == false) {
            Debug.LogError("Tried to use action, but didn't have enough mana");
        }

        if(CanSelect(infoSelection) == false) {
            Debug.LogError("Tried to use action, but it's not a valid selection");
        }

        // IMPORTANT - since we're pushing these effects onto the stack, we'll want to 
        //             push them in reverse order so that we will evaluate the most recently pushed effect first



        //Finally, add an ending marker after the ability is executed
        PushEndingMarker();

        //Let the type of this action dictate the behaviour and push all relevant effects onto the stack
        type.UseAction();

        //Then, add a starting marker before the abilities' effects
        PushStartingMarker();

        //Fourth, pay the cooldown
        PayCooldown();

        //Third, pay the fatigue
        PayFatigue();

        //Second pay the mana cost for the action
        PayManaCost();

        //First pay the action points
        PayActionPoints();
    }


    public void Execute() {

        //Push a reference to each of our clauses onto the stack so they may be evaluated later
        ContAbilityEngine.PushClauses(lstClauses);
    }

    //Determine if the ability could be used targetting the passed indices (Note: doesn't include mana check)
    public virtual bool CanSelect(SelectionSerializer.SelectionInfo selectionInfo) {// Maybe this doesn't need to be virtual

        //First, check if we're at least alive
        if(chrSource.bDead == true) {
            return false;
        }

        //Check that we're in a readiness state (with enough usable actions left)
        if(chrSource.curStateReadiness.CanSelectAction(this) == false) {
            //Debug.Log("Not in a state where we can use this action");
            return false;
        }

        //Check that the ability isn't on cooldown
        if(skillslot.IsOffCooldown() == false) {
            //Debug.Log ("Ability on cd");
            return false;
        }

        if(lstClauses[iDominantClause].IsSelectable(selectionInfo) == false) {
            Debug.Log("This selection would make the dominant clause invalid");
            return false;
        }

        return true;
    }

    //Determine if the ability can be executed with the given selection parameters - this is more allowable
    //  since we just want to ensure a prepped ability will not fizzle if it can at least do something
    public bool CanExecute(SelectionSerializer.SelectionInfo selectionInfo) {

        //First, check if we're at least alive
        if(chrSource.bDead == true) {
            return false;
        }

        //Check that we're in a readiness state (with enough usable actions left)
        if(chrSource.curStateReadiness.CanSelectAction(this) == false) {
            //Debug.Log("Not in a state where we can use this action");
            return false;
        }

        //Finally, check if there's at least one valid target to execute on - some ability clauses
        // without targets won't make sense to execute if the dominant clause has no targets.
        //  E.g. - a vampire bite's healing clause wouldn't make sense to execute if its
        //         damage clause has no viable target
        if(lstClauses[iDominantClause].HasFinalTarget(selectionInfo) == false) {
            Debug.Log("This selection would make the dominant clause invalid");
            return false;
        }

        return true;
    }

    //Determine if the ability should continue as a channel.  If the character dies, or has no valid targets for completing
    //  the channel targetting, then we can cancel it
    public bool CanCompleteAsChannel(SelectionSerializer.SelectionInfo selectionInfo) {

        //First, check if we're at least alive
        if(chrSource.bDead == true) {
            return false;
        }

        //Finally, check if there's at least one valid target to execute on - some ability clauses
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

        public ClausePayMana(Action _act) : base(_act) {
        }

        public override string GetDescription() {
            return string.Format("Paying mana for skill");
        }

        public override void ClauseEffect() {

            ContAbilityEngine.PushSingleExecutable(new ExecChangeMana(action.chrSource, action.chrSource.plyrOwner, Mana.ConvertToCost(action.parCost.Get())));

        }

    };

    class ClausePayCooldown : ClauseSpecial {

        public ClausePayCooldown(Action _act) : base(_act) {
        }

        public override string GetDescription() {
            return string.Format("Paying cooldown for skill");
        }

        public override void ClauseEffect() {

            ContAbilityEngine.PushSingleExecutable(new ExecChangeCooldown(action.chrSource, action, action.nCooldownInduced));

        }

    };

    class ClausePayFatigue : ClauseSpecial {

        public ClausePayFatigue(Action _act) : base(_act) {
        }

        public override string GetDescription() {
            return string.Format("Paying fatigue for skill");
        }

        public override void ClauseEffect() {

            ContAbilityEngine.PushSingleExecutable(new ExecChangeFatigue(action.chrSource, action.chrSource, action.nFatigue, false));

        }

    };

    class ClausePayActionPoints : ClauseSpecial {

        public ClausePayActionPoints(Action _act) : base(_act) {

        }

        public override string GetDescription() {
            return string.Format("Paying action points");
        }

        public override void ClauseEffect() {
            ContAbilityEngine.PushSingleExecutable(new ExecPayActionPoints(action.chrSource, action.chrSource, action));
        }
    };

    class ClauseStartAbility : ClauseSpecial {

        public ClauseStartAbility(Action _act) : base(_act) {
        }

        public override string GetDescription() {
            return string.Format("Starting marker for this skill");
        }

        public override void ClauseEffect() {

            ContAbilityEngine.PushSingleExecutable(new ExecStartAbility(action.chrSource, action));

        }

    };

    class ClauseEndAbility : ClauseSpecial {

        public ClauseEndAbility(Action _act) : base(_act) {
        }

        public override string GetDescription() {
            return string.Format("Ending marker for this skill");
        }

        public override void ClauseEffect() {

            ContAbilityEngine.PushSingleExecutable(new ExecEndAbility(action.chrSource, action));

        }

    };

}
