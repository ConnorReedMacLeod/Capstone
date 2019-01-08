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



    public override void Execute() {

        //We assume that we have just come from choosing an action, so get that character
        Chr chrNextToAct = ContTurns.Get().GetNextActingChr();

        ContTarget.Get().CancelTar(); // If you're in the middle of targetting an ability, cancel that targetting
        chrNextToAct.LockTargetting(); // Lock that character so they can't change ability selection

        if (chrNextToAct.bSetAction == true) {
            sLabel = chrNextToAct.sName + " is using " + chrNextToAct.arActions[chrNextToAct.nUsingAction].sName;
            chrNextToAct.ExecuteAction();

        } else {
            sLabel = chrNextToAct.sName + " has not set an Action";

            if (chrNextToAct.nQueuedFatigue == 0 && chrNextToAct.nCurActionsLeft == chrNextToAct.nMaxActionsLeft) {
                //If the character has only used cantrip abilities that would don't increase fatigue,
                //(so only cantrips with +0 fatigue), then we set the character to use a rest action

                //We also set the QueuedFatigue so that the character doesn't go again immediately
                chrNextToAct.nQueuedFatigue = 3;

                chrNextToAct.SetRestAction();
                chrNextToAct.ExecuteAction();
            }

            // Finished any cleanup we need to do for a character ending their ability selection
            chrNextToAct.FinishSelectionPhase();

        }

        fDelay = 2.0f;

        if(ContTurns.Get().GetNextActingChr() == null) {
            // Then there are no more characters that need to go after us this turn
            ContTurns.Get().SetTurnState(ContTurns.STATETURN.TURNEND);
        } else {
            // Otherwise, we have a character we need to select actions for
            ContTurns.Get().SetTurnState(ContTurns.STATETURN.CHOOSEACTIONS);
        }

        base.Execute();
    }
}
