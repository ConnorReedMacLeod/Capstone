using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionFireball : Action {



	public ActionFireball(Chr _chrOwner): base(1, _chrOwner){//number of target arguments

		//Since the base constructor initializes this array, we can start filling it
		arArgs [0] = new TargetArgChr ((own, tar) => own.plyrOwner != tar.plyrOwner);

		sName = "Fireball";
		type = ActionType.ACTIVE;

        //Physical, Mental, Energy, Blood, Effort
		arCost = new int[]{0,0,1,0,0};

		nCd = 6;
        nFatigue = 4;
        nActionCost = 1;

        sDescription = "Deal 5 damage to target character";

		SetArgOwners ();
	}

	override public void Execute(){
		//It's a bit awkward that you have to do this typecasting, 
		// but at least it's eliminated from the targetting lambda
		Chr tar = ((TargetArgChr)arArgs [0]).chrTar;

        stackClauses.Push(new Clause() {
            fExecute = () => {
                Debug.Log("This Fireball Clause put an ExecDamage on the stack");
                ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                    chrSource = this.chrSource,
                    chrTarget = tar,
                    nDamage = 10,
                    fDelay = 1.0f,
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

                    fDelay = 1.0f,
                    sLabel = "Applying Burn Effect"
                });
            }
        });


        //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;

        base.Execute ();
	}

}
