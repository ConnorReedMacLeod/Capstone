﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Can create executables like ...= new ExecGainArmour(){chrTarget = ..., nAmount = ...};

public class ExecGainPermArmour : Executable {

    public int nArmour;



    //Note:: This section should be copy and pasted for each type of executable
    //       We could do a gross thing like 
    //        this.GetType().GetMember("subAllPreTrigger", BindingFlags.Public |BindingFlags.Static);
    //       in a single base implementation of GetPreTrigger, but this should be slower and less reliable
    public static Subject subAllPreTrigger = new Subject();
    public static Subject subAllPostTrigger = new Subject();

    public override Subject GetPreTrigger() {
        return subAllPreTrigger; //Note this auto-resolves to the static member
    }
    public override Subject GetPostTrigger() {
        return subAllPostTrigger;
    }
    // This is the end of the section that should be copied and pasted


    public override void Execute() {

        //NOTE - THIS IS FOR PERMANENT ARMOUR
        chrTarget.AddArmour(nArmour);

        base.Execute();
    }
}