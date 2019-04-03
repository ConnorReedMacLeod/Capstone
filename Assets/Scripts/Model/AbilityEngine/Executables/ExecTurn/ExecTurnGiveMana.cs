using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecTurnGiveMana : Executable {


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

    public void GiveMana() {

        Mana.MANATYPE manaGen = ContMana.Get().GetTurnStartMana();
        Debug.Log("Gave out " + manaGen + " mana");

        //Give the mana to each player
        for (int i = 0; i < Match.Get().nPlayers; i++) {
            Match.Get().arPlayers[i].mana.ChangeMana(manaGen);
        }
    }

    public override void ExecuteEffect() {

        GiveMana();

        ContTurns.Get().SetTurnState(ContTurns.STATETURN.TURNSTART);

        sLabel = "Giving Mana";
        fDelay = ContTurns.fDelayTurnAction;

    }
}
