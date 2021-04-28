using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Can create executables like ...= new Exec(){chrTarget = ..., nDamage = ...};

public class ExecDealDamage : ExecChr {

    public Damage dmg;

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

        Debug.Assert(chrTarget == dmg.chrTarget);

        //Apply the stored damage to the target
        chrTarget.TakeDamage(dmg);

        sLabel = chrSource.sName + " is harming " + chrTarget.sName + " for " + dmg.Get();

        fDelay = ContTurns.fDelayMinorAction;
    }

    //If you're always expecting the same base amount, then just have an interface that just requires an integer for the damage amount
    public ExecDealDamage(Chr _chrSource, Chr _chrTarget, int nBaseDamage) : this(_chrSource, _chrTarget, (__chrSource, __chrTarget) => nBaseDamage) {
    }

    public ExecDealDamage(Chr _chrSource, Chr _chrTarget, Damage.FuncBaseDamage funcBaseDamage) : base(_chrSource, _chrTarget) {
        dmg = new Damage(_chrSource, _chrTarget, funcBaseDamage);
    }

    public ExecDealDamage(Chr _chrSource, Chr _chrTarget, Damage _dmg) : base(_chrSource, _chrTarget) {
        dmg = new Damage(_dmg);

        dmg.chrSource = _chrSource;
        dmg.chrTarget = _chrTarget;
    }

    public ExecDealDamage(ExecDealDamage other) : base(other){
        dmg = new Damage(other.dmg);
    }

}
