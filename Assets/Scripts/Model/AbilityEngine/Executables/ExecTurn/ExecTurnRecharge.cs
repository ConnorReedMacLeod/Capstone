using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecTurnRecharge : Executable {


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



    //Want to stack up a recharge executable (change fatigue/channeltime) for each character one by one
    public void RechargeChars() {

        for (int i = 0; i < Match.Get().nPlayers; i++) {
            for (int j = 0; j < Player.MAXCHRS; j++) {
                if (Match.Get().arChrs[i][j] == null) {
                    continue; // A character isn't actually here (extra space for characters)
                }

                if (Match.Get().arChrs[i][j].bDead) {
                    continue; //The character's already dead
                }

                //Ask the character's readiness state to tick down its fatigue (or channeltimer as the case may be)
                Match.Get().arChrs[i][j].curStateReadiness.Recharge();

            }
        }
    }

    public override void ExecuteEffect() {

        RechargeChars();

        ContTurns.Get().SetTurnState(ContTurns.STATETURN.READY);

        sLabel = "Reducing Fatigue/ChannelTimes";
        fDelay = 0.5f;

        base.ExecuteEffect();
    }
}
