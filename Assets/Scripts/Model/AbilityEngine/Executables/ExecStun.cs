﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Can create executables like ...= new Exec(){chrTarget = ..., nDamage = ...};

public class ExecStun : Executable {

    public LibFunc.Get<int> GetDuration;



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

        //First interrupt the character if they're channeling
        chrTarget.curStateReadiness.InterruptChannel();

        //Create a new stun state to let our character transition to
        StateStunned newState = new StateStunned(chrTarget, GetDuration());

        //Transition to the new state
        chrTarget.SetStateReadiness(newState);

        base.ExecuteEffect();
    }

}
