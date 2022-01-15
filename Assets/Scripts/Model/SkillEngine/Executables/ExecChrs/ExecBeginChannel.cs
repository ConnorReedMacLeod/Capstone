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

    Skill skillChannel;

    public override void ExecuteEffect() {
        Debug.Log("Beginning of ExecBeginChannel.ExecuteEffect");

        TypeUsageChannel typeChannel = (TypeUsageChannel)skillChannel.typeUsage;

        //Ask the soulbehaviour to make a copy of itself with the skill it represents
        StateChanneling newState = new StateChanneling(chrTarget, typeChannel.nStartChannelTime, typeChannel.soulBehaviour.GetCopy(skillChannel));

        Debug.Log("Before SetStateReadiness");

        //We don't need to perform any real action on starting channeling other than changing our readiness state so that the 
        // soulchannel effect can be applied (and do any on-application effects if necessary)
        chrTarget.SetStateReadiness(newState);

        fDelay = ContTime.fDelayTurnSkill;
        sLabel = chrTarget.sName + " is beginning their channel";
        Debug.Log("After SetStateReadiness");

    }

    public ExecBeginChannel(Chr _chrSource, Chr _chrTarget, Skill _skillChannel) : base(_chrSource, _chrTarget) {
        skillChannel = _skillChannel;
    }

    public ExecBeginChannel(ExecBeginChannel other) : base(other) {

    }
}
