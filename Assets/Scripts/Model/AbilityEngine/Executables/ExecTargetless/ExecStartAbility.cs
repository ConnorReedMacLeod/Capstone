using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecStartAbility : Executable {

    public Action act; //The action that this marker represents the start of

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
        //TODO:: In the future, this marker could check if the ability targetting is still legal,
        //       and if it's not, pop off items from the top of the stack until it reaches the
        //       end marker to cancel all effects of the ability
        return true;

    }

    public ExecStartAbility(Chr _chrSource, Action _act): base(_chrSource) {
        act = _act;
        chrSource = act.chrSource;
    }

    public override void ExecuteEffect() {
        //Debug.Log("Notifying that an ability has started");

        //Notify everyone that we're just about to start the effects of an ability
        chrSource.subPreExecuteAbility.NotifyObs(chrSource, act);
        Chr.subAllPreExecuteAbility.NotifyObs(chrSource, act);
    }

    public ExecStartAbility(ExecStartAbility other) : base(other) {
        act = other.act;
    }
    
}