using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecTurnChooseSkills : Executable {



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
        //Can't invalidate a turn skill
        return true;
    }


    //All that chooseskills needs to do is store the selection information for the
    // chosen skill so that ContTurns->FinishedTurnPhase can post this information to the master
    public override void ExecuteEffect() {

        //Get the character that is currently set to be acting next
        Chr chrActing = ContTurns.Get().GetNextActingChr();

        //Fill the current matchinputtofill with a new blank skill selection request (just set to the character passing their turn)
        ContSkillEngine.Get().matchinputToFillOut = new InputSkillSelection(chrActing.plyrOwner, chrActing, chrActing.skillRest.skillslot);

        //Note that since matchinputToFillOut is 'raised' by being non-null, then we'll stop evaluating executables until we 
        //  receive completed input from the active player
    }

    public ExecTurnChooseSkills(Chr _chrSource) : base(_chrSource) {

    }

    public ExecTurnChooseSkills(ExecTurnChooseSkills other) : base(other) {

    }

}
