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

    public SoulChannel soulChannel; // Stores behaviour for how this channel should work (if this is a channel)
    public int nActionCost; // How many action 'points' this ability uses - cantrips would cost 0

    public Chr chrSource;

    public bool bCharges;
    public int nCharges;
    public int nCurCharges;

    public bool bProperActive;  //Usually true - only false for non-standard actions that shouldn't 
                                // switch the character sprite to an acting portrait (for example)


	public Property<int[]> parCost;

    public List<Clause> lstClauses = new List<Clause>();
    public List<Clause> lstClausesOnEquip = new List<Clause>();
    public List<Clause> lstClausesOnUnequip = new List<Clause>();

    public int iBaseClause;

    public Subject subAbilityChange = new Subject();

    public Action(Chr _chrOwner, int _iBaseClause) {
        chrSource = _chrOwner;
        iBaseClause = _iBaseClause;

        bProperActive = true;

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

        ContAbilityEngine.PushClauses(lstClausesOnEquip);

    }

    //What should happen when this action is remove from the list of abilities
    public virtual void OnUnequip() {

        ContAbilityEngine.PushClauses(lstClausesOnUnequip);

    }

    public void PayManaCost() {

        ContAbilityEngine.PushSingleClause(new Clause() {

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
        ContAbilityEngine.Get().AddClause(
            new ClauseSpecial(this) {
                lstExec = new List<Executable> { new ExecEndAbility(this) }
            });

        //Let the type of this action dictate the behaviour and push all relevant effects onto the stack
        type.UseAction(lstTargettingIndices);

        //Add a marker on top for where the ability starts
        ContAbilityEngine.Get().AddClause(
            new ClauseSpecial(this) {
                lstExec = new List<Executable> { new ExecStartAbility(this) }
            });


    }


    public void Execute() {

        //Give our list of clauses to evaluate to the engine to push onto the stack
        ContAbilityEngine.PushClauses(lstClauses);
    }

    //Check if the owner is alive and that the proposed targets are legal
    public virtual bool LegalTargets(int[] lstTargettingIndices) {

        if (chrSource.bDead) {
            Debug.Log("The character source is dead");
            return false;
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
			//Debug.Log ("Ability on cd");
			return false;
		}

        //Check that we're in a readiness state (with enough usable actions left)
        if (!chrSource.curStateReadiness.CanSelectAction(this)) {
            //Debug.Log("Not in a state where we can use this action");
            return false;
        }

        if(LegalTargets(lstTargettingIndices) == false) {
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

}
