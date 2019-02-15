using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecTurnExecuteAction : Executable {


    //Note:: This section should be copy and pasted for each type of executable
    //       We could do a gross thing like 
    //        this.GetType().GetMember("subAllPreTrigger", BindingFlags.Public |BindingFlags.Static);
    //       in a single base implementation of GetPreTrigger, but this should be slower and less reliable
    public static Subject subAllPreTrigger = new Subject();
    public static Subject subAllPostTrigger = new Subject();

    //Keep a list of the replacement effects for this executable type
    public static List<Replacement> lstAllReplacements = new List<Replacement>();
    public static List<Replacement> lstAllFullReplacements = new List<Replacement>();

    public override Subject GetPreTrigger() {
        return subAllPreTrigger; //Note this auto-resolves to the static member
    }
    public override Subject GetPostTrigger() {
        return subAllPostTrigger;
    }
    public override List<Replacement> GetReplacements() {
        return lstAllReplacements;
    }
    public override List<Replacement> GetFullReplacements() {
        return lstAllFullReplacements;
    }
    // This is the end of the section that should be copied and pasted


    public override bool isLegal() {
        //Can't invalidate a turn action
        return true;
    }

    public override void ExecuteEffect() {

        //We assume that we have just come from choosing an action, so get that character
        Chr chrNextToAct = ContTurns.Get().GetNextActingChr();

        ContTarget.Get().CancelTar(); // If you're in the middle of targetting an ability, cancel that targetting
        chrNextToAct.LockTargetting(); // Lock that character so they can't change ability selection

        if (chrNextToAct.bSetAction == true) {
            //If the character has prepped an action, then we should use it
            sLabel = chrNextToAct.sName + " is using " + chrNextToAct.arActions[chrNextToAct.nUsingAction].sName;
            chrNextToAct.ExecuteAction();

        } else {
            sLabel = chrNextToAct.sName + " has not set an Action";

            if (chrNextToAct.nFatigue == 0 && 
                ((StateReady)(chrNextToAct.curStateReadiness)).nCurActionsLeft == chrNextToAct.nMaxActionsLeft) {
                //If the character has 0 fatigue after not using any actives/channels
                // then we set the character to use a rest action

                //We set up a rest action (that will give 3 fatigue) so that we don't have the same character going next turn
                chrNextToAct.SetRestAction();
                chrNextToAct.ExecuteAction();
            }

            //Then move this character to a fatigued state
            chrNextToAct.SetStateReadiness(new StateFatigued(chrNextToAct));
        }

        fDelay = ContTurns.fDelayStandard;

        //Move back to choosing actions (in case there's more actions to be chosen)
        ContTurns.Get().SetTurnState(ContTurns.STATETURN.CHOOSEACTIONS);

    }
}
