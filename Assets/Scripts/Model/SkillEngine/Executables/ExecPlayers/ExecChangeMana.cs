using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Can create executables like ...= new Exec(){chrTarget = ..., nDamage = ...};

public class ExecChangeMana : ExecPlayer {

    //One of the following can be set - whichever is nicer for the situation
    public Mana manaChange;

    public Mana.MANATYPE manaType;
    public int nAmount;

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

    public ExecChangeMana(Chr _chrSource, Player _plyrTarget, Mana _manaChange) : base(_chrSource, _plyrTarget) {
        plyrTarget = _plyrTarget;
        manaChange = _manaChange;
    }

    public ExecChangeMana(Chr _chrSource, Player _plyrTarget, Mana.MANATYPE _manaType, int _nAmount = 1) : base(_chrSource, _plyrTarget) {
        plyrTarget = _plyrTarget;
        manaType = _manaType;

        nAmount = _nAmount;
    }

    public ExecChangeMana(ExecChangeMana other) : base(other) {
        manaChange = new Mana(other.manaChange);

        manaType = other.manaType;
        nAmount = other.nAmount;
    }

    public override bool isLegal() {
        //Currently, there is no way to invalidate giving mana
        return true;
    }

    public override void ExecuteEffect() {

        if(manaChange == null) {
            //If no instance of mana was added, then change the single amount of mana passed
            plyrTarget.manapool.ChangeMana(manaType, nAmount);
        } else {
            //But if an instance of mana was specified, then use that to change mana
            plyrTarget.manapool.ChangeMana(manaChange);
        }

        fDelay = ContTurns.fDelayMinorSkill;
        sLabel = "Changing mana for player " + plyrTarget.id;

    }

}
