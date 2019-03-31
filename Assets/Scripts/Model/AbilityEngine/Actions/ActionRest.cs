using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionRest : Action {

    public int nRestFatigue;

	public ActionRest(Chr _chrOwner): base(0, _chrOwner){//number of target arguments

		sName = "Rest";
		type = new TypeCantrip(this);

        chrSource = _chrOwner;

        parCost = new Property<int[]>(new int[]{0,0,0,0,0});

		nCd = 0;
        nFatigue = 0;

        bProperActive = false; //This is a special action that shouldn't change our character's sprite to the active

        nRestFatigue = 3;

		sDescription1 = _chrOwner + " has finished selecting abilities for the turn.";

	}

	override public void Execute(int[] lstTargettingIndices) {

		Debug.Log (chrSource.sName + " is resting");
        stackClauses.Push(new Clause() {
            fExecute = () => {
                //Check if the character has any fatigue already
                if (chrSource.nFatigue == 0) {
                    //If not, then give them three fatigue
                    ContAbilityEngine.Get().AddExec(new ExecChangeFatigue() {
                        chrSource = this.chrSource,
                        chrTarget = this.chrSource,

                        nAmount = this.nRestFatigue,

                        fDelay = ContTurns.fDelayStandard,
                        sLabel = this.chrSource.sName + " is resting"
                    });
                }
            }
        });

    }

}
