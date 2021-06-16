using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Can create executables like ...= new Exec(){chrTarget = ..., nDamage = ...};

public class ExecHeal : ExecChr {

    public Healing heal;

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

        Debug.Assert(chrTarget == heal.chrTarget);

        //Apply the stored heal to the target
        chrTarget.TakeHealing(heal);

        sLabel = chrSource.sName + " is healing " + chrTarget.sName + " for " + heal.Get();

        fDelay = ContTurns.fDelayMinorSkill;

    }

    //If you're always expecting the same base amount, then just have an interface that just requires an integer for the healing amount
    public ExecHeal(Chr _chrSource, Chr _chrTarget, int nBaseHealing) : this(_chrSource, _chrTarget, (__chrSource, __chrTarget) => nBaseHealing){
    }

    public ExecHeal(Chr _chrSource, Chr _chrTarget, Healing.FuncBaseHeal funcBaseHeal): base (_chrSource, _chrTarget) {
        heal = new Healing(_chrSource, _chrTarget, funcBaseHeal);
    }

    //Have a constructor that just accepts a premade Healing instance and copies it
    public ExecHeal(Chr _chrSource, Chr _chrTarget, Healing _heal ): base(_chrSource, _chrTarget) {
        heal = new Healing(_heal);

        heal.chrSource = _chrSource;
        heal.chrTarget = _chrTarget;
    }

    public ExecHeal(ExecHeal other) : base(other) {
        heal = new Healing(other.heal);
    }

}
