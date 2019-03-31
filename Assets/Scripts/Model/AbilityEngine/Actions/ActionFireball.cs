using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionFireball : Action {

    public Damage dmg;
    public int nBaseDamage;

    public ActionFireball(Chr _chrOwner): base(1, _chrOwner){//number of target arguments

		//Since the base constructor initializes this array, we can start filling it
		arArgs [0] = new TargetArgChr ((own, tar) => own.plyrOwner != tar.plyrOwner);

		sName = "Fireball";
        sDisplayName = "Fireball";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[]{0,0,1,0,0});

		nCd = 6;
        nFatigue = 4;
        nActionCost = 1;

        nBaseDamage = 5;
        //Create a base Damage object that this action will apply
        dmg = new Damage(this.chrSource, null, nBaseDamage);

        sDescription1 = "Deal 5 damage to the chosen character.";

		SetArgOwners ();
	}

	override public void Execute(int[] lstTargettingIndices) {

        Chr tar = Chr.GetTargetByIndex(lstTargettingIndices[0]);

        stackClauses.Push(new Clause() {
            fExecute = () => {
                Debug.Log("This Fireball Clause put an ExecDamage on the stack");

                //Make a copy of the damage object to give to the executable
                Damage dmgToApply = new Damage(dmg);
                //Give the damage object its target
                dmgToApply.SetChrTarget(tar);

                ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                    chrSource = this.chrSource,
                    chrTarget = tar,
                    dmg = dmgToApply,
                    fDelay = ContTurns.fDelayStandard,
                    sLabel = "Fireballing"
                });
            }
        });

        stackClauses.Push(new Clause() {
            fExecute = () => {
                Debug.Log("Fireball's second clause put an ExecApplySoul on the stack");
                ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
                    chrSource = this.chrSource,
                    chrTarget = tar,

                    funcCreateSoul = (_chrSource, _chrTarget) => {
                        return new SoulBurning(_chrSource, _chrTarget);
                    },

                    fDelay = ContTurns.fDelayStandard,
                    sLabel = "Applying Burn Effect"
                });
            }
        });

	}

}
