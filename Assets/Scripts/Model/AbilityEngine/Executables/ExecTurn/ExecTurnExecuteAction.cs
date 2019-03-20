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

        //Check if we're resting
        if (ContAbilitySelection.Get().nSelectedAbility == Chr.idResting &&
            chrNextToAct.nFatigue > 0) {
            //If we have a rest action selected, but we have accrued some fatigue

            //Then no action needs to be taken, just leave the character in a fatigued state
            sLabel = chrNextToAct.sName + " is finished selecting abilities for the turn";
            chrNextToAct.SetStateReadiness(new StateFatigued(chrNextToAct));
        }else { 
        
            //If here, then we're doing a normal action (or a proper rest for 3 turns)

            sLabel = chrNextToAct.sName + " is using " + chrNextToAct.arActions[ContAbilitySelection.Get().nSelectedAbility].sName;

            chrNextToAct.ExecuteAction(ContAbilitySelection.Get().nSelectedAbility, ContAbilitySelection.Get().lstSelectedTargets);
        }

        fDelay = ContTurns.fDelayStandard;

        //Move back to choosing actions (in case there's more actions to be chosen)
        ContTurns.Get().SetTurnState(ContTurns.STATETURN.CHOOSEACTIONS);

    }
}
