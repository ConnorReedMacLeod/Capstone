using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This Executable does nothing on resolution - it's useful for if you want to nullify some replaced effect
public class ExecNull : ExecTargetless {
    //TODO:: Consider if all of these lists and triggers are even necessary
    //         I'm fairly sure they wouldn't be used, but I'm not sure I can just delete them

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

    public override bool isLegal() {
        //Can't invalidate a Null action
        return true;
    }

    public override void ExecuteEffect() {

        this.sLabel = "Nothing happens";

    }

    public ExecNull(ExecNull other) : base(other) {

    }

    public override Executable MakeCopy() {
        return new ExecNull(this);
    }

}
