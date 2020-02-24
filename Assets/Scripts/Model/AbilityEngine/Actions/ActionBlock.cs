using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBlock : Action {

    public ActionBlock(Chr _chrOwner) : base(_chrOwner, 0) {

        sName = "DeclareBlocker";
        sDisplayName = "Declare Vanguard";

        type = new TypeCantrip(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 0 });

        nCd = 1; //This might have issues if you can reduce cooldowns a lot - don't want looping
        nFatigue = 0;
        nActionCost = 0;
        

    }

    class Clause1 : ClauseSpecial {

        public Clause1(Action _act) : base(_act) {

        }

        public override string GetDescription() {

            //TODO - eventually figure out how I'm gonna dynamically generate the text targets
            return "Become the Vanguard.  (Melee attacks can only target the Vanguard)";
        }

        public override void ClauseEffect() {
            ContAbilityEngine.PushSingleExecutable(new ExecBecomeBlocker(action.chrSource, action.chrSource) {
                sLabel = "Becoming the vanguard"
            });
        }

    }

}