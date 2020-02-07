using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecTurnReady : Executable {


    //Note:: This section should be copy and pasted for each type of executable
    //       We could do a gross thing like 
    //        this.GetType().GetMember("subAllPreTrigger", BindingFlags.Public |BindingFlags.Static);
    //       in a single base implementation of GetPreTrigger, but this should be slower and less reliable
    public static Subject subAllPreTrigger = new Subject(Subject.SubType.ALL);
    public static Subject subAllPostTrigger = new Subject(Subject.SubType.ALL);

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

    public void ReadyAll() {
        //Loop through all characters and transition any 0 fatigue characters to be ready

        for (int i = 0; i < Match.Get().nPlayers; i++) {
            for (int j = 0; j < Player.MAXCHRS; j++) {
                if (Match.Get().arChrs[i][j] == null) {
                    continue; // A character isn't actually here (extra space for characters)
                }

                //Call the character's Ready action - they'll decide what to do
                Match.Get().arChrs[i][j].curStateReadiness.Ready();

            }
        }
    }

    public override bool isLegal() {
        //Can't invalidate a turn action
        return true;
    }

    public override void ExecuteEffect() {

        ReadyAll();

        sLabel = "Readying Characters";
        fDelay = ContTurns.fDelayTurnAction;

    }

    public ExecTurnReady(ExecTurnReady other) : base(other) {

    }
}