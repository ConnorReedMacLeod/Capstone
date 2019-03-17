using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action { //This should probably be made abstract

    public int id;

    public int nArgs; // Note that this should only ever be 0 or 1
    public TargetArg[] arArgs;
    public string sName;
    public TypeAction type;

    public int nCd;
    public int nCurCD;
    public int nFatigue;

    public SoulChannel soulChannel; // Stores behaviour for how this channel should work (if this is a channel)
    public int nActionCost; // How many action 'points' this ability uses - cantrips would cost 0

    public Chr chrSource;

    public bool bCharges;
    public int nCharges;
    public int nCurCharges;

    public string sDescription;
    public string sExtraDescription;

    public Property<int[]> parCost;

    public Stack<Clause> stackClauses = new Stack<Clause>();

    public Subject subAbilityChange = new Subject();

    public Action(int _nArgs, Chr _chrOwner) {
        nArgs = _nArgs;
        chrSource = _chrOwner;


        arArgs = new TargetArg[nArgs];

    }

    public void SetArgOwners() {
        for (int i = 0; i < nArgs; i++) {
            arArgs[i].setOwner(chrSource);
        }
    }


    public void ChangeCD(int _nChange) {
        if (_nChange + nCurCD < 0) {
            // Don't let reductions go negative
            nCurCD = 0;
        } else {
            nCurCD += _nChange;
            subAbilityChange.NotifyObs();

        }
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

        while (stackClauses.Count != 0) {
            //Add each clause in this ability to the stack
            ContAbilityEngine.Get().AddClause(stackClauses.Pop());
        }

    }

    //What should happen when this action is remove from the list of abilities
    public virtual void OnUnequip() {

        while (stackClauses.Count != 0) {
            //Add each clause in this ability to the stack
            ContAbilityEngine.Get().AddClause(stackClauses.Pop());
        }

    }

    public void PayManaCost() {

        ContAbilityEngine.Get().AddClause(new Clause() {

            fExecute = () => {
                //Pay for the Action
                ContAbilityEngine.Get().AddExec(new ExecChangeMana(chrSource.plyrOwner, Mana.ConvertToCost(parCost.Get())) {
                    chrSource = this.chrSource,
                    chrTarget = null,
                });
            }
        });
    }

    public void PayCooldown() {
        ContAbilityEngine.Get().AddClause(new Clause() {

            fExecute = () => {
                //Increase this Action's cooldown
                ContAbilityEngine.Get().AddExec(new ExecChangeCooldown() {
                    chrSource = this.chrSource,
                    chrTarget = this.chrSource,

                    actTarget = this,
                    nAmount = nCd
                });
            }
        });
    }

    public void PayFatigue() {

        ContAbilityEngine.Get().AddClause(new Clause() {

            fExecute = () => {
                //Increase the character's fatigue
                ContAbilityEngine.Get().AddExec(new ExecChangeFatigue {
                    chrSource = this.chrSource,
                    chrTarget = this.chrSource,

                    nAmount = nFatigue
                });
            }
        });

    }

    public bool CanPayMana() {
        //Check if you have enough mana
        if (chrSource.plyrOwner.mana.HasMana(parCost.Get()) == false) {
            Debug.Log("Not enough mana");
            return false;
        }
        return true;
    }

    //Use the selected action with the supplied targets
    public void UseAction(int[] lstTargettingIndices) {

        if(CanPayMana() == false) {
            Debug.LogError("Tried to use action, but didn't have enough mana");
        }

        if (CanActivate(lstTargettingIndices) == false) {
            Debug.LogError("Tried to use action, but it's not a valid selection");
        }

        //First pay the mana cost for the action
        PayManaCost();

        //Add a marker for the end of the ability below all of the effects for this ability
        ContAbilityEngine.Get().AddClause(new ClauseEndAbility(this));

        //Let the type of this action dictate the behaviour and push all relevant effects onto the stack
        type.UseAction(lstTargettingIndices);

        //Add a marker on top for where the ability starts
        ContAbilityEngine.Get().AddClause(new ClauseStartAbility(this));


    }

    // Perform the actual effect this action should do
    // This is the main effect of the action for actives/cantrips
    //  and is the completion action for cantrips
    public virtual void Execute(int[] lstTargettingIndices) {
        //By default do nothing - just override this to make the action do something
    }

    //Check if the owner is alive and that the proposed targets are legal
    public virtual bool LegalTargets(int[] lstTargettingIndices) {

        if (chrSource.bDead) {
            Debug.Log("The character source is dead");
            return false;
        }

        if (lstTargettingIndices.Length != arArgs.Length) {
            Debug.Log("Note - supplied list of targets isn't the correct length");
        }

        for (int i = 0; i < arArgs.Length; i++) {
            if (arArgs[i].WouldBeLegal(lstTargettingIndices[i]) == false) {
                Debug.Log("Argument " + i + " would be invalid");
                return false;
            }
        }

        return true;
    }


    //Determine if the ability could be used targetting the passed indices (Note: doesn't include mana check)
	public virtual bool CanActivate(int[] lstTargettingIndices) {// Maybe this doesn't need to be virtual

		//Check that the ability isn't on cooldown
		if (nCurCD != 0) {
			Debug.Log ("Ability on cd");
			return false;
		}

        //Check that we're in a readiness state (with enough usable actions left)
        if (!chrSource.curStateReadiness.CanSelectAction(this)) {
            Debug.Log("Not in a state where we can use this action");
            return false;
        }

        if(LegalTargets(lstTargettingIndices) == false) {
            Debug.Log("Targets aren't legal");
            return false;
        }

		
		return true;
	}
		
}
