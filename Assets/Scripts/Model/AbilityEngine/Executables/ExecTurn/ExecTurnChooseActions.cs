using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecTurnChooseActions : Executable {



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




    public override void ExecuteEffect() {

        //First, test if we actually have any character who is ready to act right now
        if(ContTurns.Get().GetNextActingChr() == null) {

            //If no character is in the ready state, then we'll move to the end of turn
            ContTurns.Get().SetTurnState(ContTurns.STATETURN.TURNEND);

        } else {

            //If we do have a character who can act, then set them up to be able to act

            //Ensure only the currently acting character can select actions
            ContTurns.Get().GetNextActingChr().UnlockTargetting();

            //Prepare to execute the action that they willl have selected
            ContTurns.Get().SetTurnState(ContTurns.STATETURN.EXECUTEACTIONS);

            sLabel = "Select Your Action for " + ContTurns.Get().GetNextActingChr().sName;
            fDelay = ContTurns.fDelayChooseAction;
        }

        base.ExecuteEffect();
    }
}
