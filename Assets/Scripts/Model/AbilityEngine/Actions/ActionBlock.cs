using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBlock : Action {

    public ActionBlock(Chr _chrOwner) : base(_chrOwner) {

        sName = "DeclareBlocker";
        sDisplayName = "Declare Vanguard";

        type = new TypeCantrip(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 0 });

        nCd = 1; //This might have issues if you can reduce cooldowns a lot - don't want looping
        nFatigue = 0;
        nActionCost = 0;

        sDescription1 = "Become the Vanguard.  (Melee attacks can only target the Vanguard)";

    }

    override public void Execute(int[] lstTargettingIndices) {


        stackClauses.Push(new Clause() {
            fExecute = () => {
                Debug.Log("Block's first clause put an ExecBecomeBlocker on the stack");
                ContAbilityEngine.Get().AddExec(new ExecBecomeBlocker() {
                    chrSource = this.chrSource,
                    chrTarget = this.chrSource

                });
            }
        });

    }

}