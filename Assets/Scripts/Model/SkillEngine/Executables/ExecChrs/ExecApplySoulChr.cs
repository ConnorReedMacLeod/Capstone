using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Can create executables like ...= new Exec(){chrSource = ..., chrTarget = ..., soulToCopy = ...};

public class ExecApplySoulChr : ExecChr {

    public SoulChr soulToApply;


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

        Debug.Assert(chrTarget == soulToApply.chrTarget);

        chrTarget.soulContainer.ApplySoul(soulToApply);

    }

    public ExecApplySoulChr(Chr _chrSource, Chr _chrTarget, SoulChr _soulToApply) : base(_chrSource, _chrTarget) {
        soulToApply = _soulToApply;

    }

    public ExecApplySoulChr(ExecApplySoulChr other) : base(other) {
        soulToApply = other.soulToApply;

    }

}
