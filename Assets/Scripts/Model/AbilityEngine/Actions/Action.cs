using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action { //This should probably be made abstract

    public int id; //TODO: Consider if you could make a ID<T> class that could dynamically generate new IDS as needed for the type T

    public string sName;
    public string sDisplayName;
    public TypeAction type;

    public int nCd;
    public int nCurCD;
    public int nFatigue;

    public Chr chrSource;

    public bool bCharges;
    public int nCharges;
    public int nCurCharges;

    public bool bProperActive;  //Usually true - only false for non-standard actions that shouldn't 
                                // switch the character sprite to an acting portrait (for example)
                                // like resting


    public Property<int[]> parCost;

    public List<Clause> lstClauses = new List<Clause>();
    public List<Clause> lstClausesOnEquip = new List<Clause>();
    public List<Clause> lstClausesOnUnequip = new List<Clause>();

    public int iDominantClause;

    public Subject subAbilityChange = new Subject();

    public Action(Chr _chrOwner, int _iDominantClause) {
        chrSource = _chrOwner;
        iDominantClause = _iDominantClause;

        bProperActive = true;

    }

    public void ChangeCD(int _nChange) {
        if(_nChange + nCurCD < 0) {
            // Don't let reductions go negative
            nCurCD = 0;
        } else {
            nCurCD += _nChange;
            subAbilityChange.NotifyObs();

        }
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

        if(CanActivate(infoSelection) == false) {
            Debug.LogError("Tried to use action, but it's not a valid selection");
        }

        // IMPORTANT - since we're pushing these effects onto the stack, we'll want to 
        //             push them in reverse order so that we will evaluate the most recently pushed effect first



        //Finally, add an ending marker after the abilities' executed
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

    //Check if the owner is alive and that the proposed targets are legal
    public virtual bool IsLegalSelectionInfo(SelectionSerializer.SelectionInfo selectionInfo) {

        if(chrSource.bDead) {
            Debug.Log("The character source is dead");
            return false;
        }

        //If this selection would result in the dominant clause doing nothing, then this isn't legal
        // TODO:: consider if this should be checked for all clauses?  At least one clause?
        if(lstClauses[iDominantClause].IsValidSelection(selectionInfo) == false) {
            Debug.Log("This selection would make the dominant clause invalid");
            return false;
        }


        return true;
    }


    //Determine if the ability could be used targetting the passed indices (Note: doesn't include mana check)
    public virtual bool CanActivate(SelectionSerializer.SelectionInfo selectionInfo) {// Maybe this doesn't need to be virtual

        //Check that the ability isn't on cooldown
        if(nCurCD != 0) {
            //Debug.Log ("Ability on cd");
            return false;
        }

        //Check that we're in a readiness state (with enough usable actions left)
        if(!chrSource.curStateReadiness.CanSelectAction(this)) {
            //Debug.Log("Not in a state where we can use this action");
            return false;
        }

        if(IsLegalSelectionInfo(selectionInfo) == false) {
            //Debug.Log("Targets aren't legal");
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

            ContAbilityEngine.PushSingleExecutable(new ExecChangeCooldown(action.chrSource, action, action.nCd));

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
