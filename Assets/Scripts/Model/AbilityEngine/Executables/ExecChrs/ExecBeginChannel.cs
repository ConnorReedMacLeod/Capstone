using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Can create executables like ...= new Exec(){chrTarget = ..., nDamage = ...};

public class ExecBeginChannel : ExecChr {

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

    Action actChannel;

    public override void ExecuteEffect() {
        Debug.Log("Beginning of ExecBeginChannel.ExecuteEffect");

        TypeChannel typeChannel = (TypeChannel)actChannel.type;

        StateChanneling newState = new StateChanneling(chrTarget, typeChannel.nStartChannelTime, new SoulChannel(typeChannel.soulBehaviour, actChannel));

        Debug.Log("Before SetStateReadiness");

        //We don't need to perform any real action on starting channeling other than changing our readiness state so that the 
        // soulchannel effect can be applied (and do any on-application effects if necessary)
        chrTarget.SetStateReadiness(newState);

        fDelay = ContTurns.fDelayTurnAction;
        sLabel = chrTarget.sName + " is beginning their channel";
        Debug.Log("After SetStateReadiness");

    }

    public ExecBeginChannel(Chr _chrSource, Chr _chrTarget, Action _actChannel) : base(_chrSource, _chrTarget) {
        actChannel = _actChannel;
    }

    public ExecBeginChannel(ExecBeginChannel other) : base(other) {

    }
}
