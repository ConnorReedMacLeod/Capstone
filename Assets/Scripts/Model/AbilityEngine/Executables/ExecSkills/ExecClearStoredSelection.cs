using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Can create executables like ...= new Exec(){chrTarget = ..., nDamage = ...};


/// <summary>
/// This is a game action to clear out the stored selection parameters of a channel action
/// after it has completed its execution - should only really be used when transitioning away
/// from a channeling state
/// </summary>

public class ExecClearStoredSelection : ExecSkill {

    //This really shouldn't be used since this is just a game-action

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


    public override void ExecuteEffect() {

        ((TypeChannel)skTarget.type).ClearStoredSelectionInfo();

        fDelay = ContTurns.fDelayGameEffects;
        sLabel = "Clearing stored " + skTarget.sDisplayName + "'s selections";

    }

    public ExecClearStoredSelection(Chr _chrSource, Action _skTarget) : base(_chrSource, _skTarget) {

    }

    public ExecClearStoredSelection(ExecClearStoredSelection other) : base(other) {

    }
}
