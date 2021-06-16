using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecEndSkill : Executable {

    public Skill skill; //The skill that this marker represents the end of

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
        //For now, this is just a marker to know when the end of a turn is
        //TODO:: In the future, this marker could check if the skill targetting is still legal,
        //       and if it's not, pop off items from the top of the stack until it reaches the
        //       end marker to cancel all effects of the skill
        return true;

    }

    public override void ExecuteEffect() {
        //Debug.Log("Notifying that a skill has ended");

        //Notify everyone that we've just completed the effects of a skill
        chrSource.subPostExecuteSkill.NotifyObs(chrSource, skill);
        Chr.subAllPostExecuteSkill.NotifyObs(chrSource, skill);
    }

    public ExecEndSkill(Chr _chrSource, Skill _skill) : base(_chrSource) {
        skill = _skill;
        chrSource = skill.chrSource;
    }

    public ExecEndSkill(ExecEndSkill other) : base(other) {

    }

}