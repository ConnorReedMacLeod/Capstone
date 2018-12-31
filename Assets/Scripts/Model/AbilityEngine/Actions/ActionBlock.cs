using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBlock : Action {

    public ActionBlock(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        //arArgs[0] = new TargetArgChr((own, tar) => own.plyrOwner != tar.plyrOwner); //Choose a target enemy

        sName = "Declare Blocker";
        type = ActionType.ACTIVE;

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 0 });

        nCd = 1; //This might have issues if yoiu can reduce cooldowns a lot - don't want looping
        nFatigue = 0;
        nActionCost = 0;

        sDescription = "Become the blocker";

        SetArgOwners();
    }

    override public void Execute() {


        stackClauses.Push(new Clause() {
            fExecute = () => {
                Debug.Log("Block's first clause put an ExecBecomeBlocker on the stack");
                ContAbilityEngine.Get().AddExec(new ExecBecomeBlocker() {
                    chrSource = this.chrSource,
                    chrTarget = this.chrSource

                });
            }
        });


        //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;

        base.Execute();
    }

}