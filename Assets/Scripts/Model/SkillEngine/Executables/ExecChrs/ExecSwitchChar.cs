﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Can create executables like ...= new Exec(){chrTarget = ..., nDamage = ...};

public class ExecSwitchChar : ExecChr {

    public Position.FuncGetPosition funcGetTargetPosition;

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

        //Figure out what the target position should be at the time of execution (relative to the character that's moving)
        Position posDestination = funcGetTargetPosition(chrTarget);

        //Call the Switch method in the position controller
        ContPositions.Get().SwitchChrToPosition(chrTarget, posDestination);

        sLabel = chrSource.sName + " is moving to " + posDestination.ToString();

        fDelay = ContTime.fDelayMinorSkill;
    }


    //Can construct an ExecSwitchChar with just a static Position if it'll always be the same (i.e., not dependent on being relative to where the character is moving)
    public ExecSwitchChar(Chr _chrSource, Chr _chrTarget, Position posDestination) : this(_chrSource, _chrTarget, (chrTarget) => posDestination) {

    }

    //Can construct an 

    public ExecSwitchChar(Chr _chrSource, Chr _chrTarget, Position.FuncGetPosition _funcGetPosition) : base(_chrSource, _chrTarget) {

        funcGetTargetPosition = _funcGetPosition;

    }

    public ExecSwitchChar(ExecMoveChar other) : base(other) {
        funcGetTargetPosition = other.funcGetTargetPosition;
    }

}
