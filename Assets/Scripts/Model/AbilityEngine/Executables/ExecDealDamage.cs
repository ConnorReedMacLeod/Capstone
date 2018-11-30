﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Can create executables like ...= new Exec(){chrTarget = ..., nDamage = ...};

public class ExecDealDamage : Executable {

    public Chr chrTarget;
    public int nDamage;

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
        //TODO:: Take into account armour and power/defense

        int nArmouredDamage = Mathf.Min(nDamage, chrTarget.nCurArmour);

        if (nArmouredDamage > 0) {
            chrTarget.ChangeFlatArmour(-nArmouredDamage);
        }

        int nAfterArmourDamage = nDamage - nArmouredDamage;

        if (nAfterArmourDamage > 0) {
            chrTarget.ChangeHealth(-nAfterArmourDamage);
        }

        base.Execute();
    }

}
