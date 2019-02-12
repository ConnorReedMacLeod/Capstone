using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Can create executables like ...= new Exec(){chrTarget = ..., nDamage = ...};

public class ExecChangeMana : Executable {

    public Player plyrTarget;

    //One of the following can be set - whichever is nicer for the situation
    public int[] arnAmount;

    public Mana.MANATYPE manaType;
    public int nAmount;

    //Note:: This section should be copy and pasted for each type of executable
    //       We could do a gross thing like 
    //        this.GetType().GetMember("subAllPreTrigger", BindingFlags.Public |BindingFlags.Static);
    //       in a single base implementation of GetPreTrigger, but this should be slower and less reliable
    public static Subject subAllPreTrigger = new Subject();
    public static Subject subAllPostTrigger = new Subject();

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

    public ExecChangeMana(Player _plyrTarget, int[] _arnAmount) {
        plyrTarget = _plyrTarget;
        arnAmount = _arnAmount;
    }

    public ExecChangeMana(Player _plyrTarget, Mana.MANATYPE _manaType, int _nAmount = 1) {
        plyrTarget = _plyrTarget;
        manaType = _manaType;

        nAmount = _nAmount;
    }

    public override void ExecuteEffect() {

        //Double check that there is no targetted character
        Debug.Assert(chrTarget == null);

        if (arnAmount == null) {
            //If no array of mana was added, then add the single amount of mana passed
            plyrTarget.mana.AddMana(manaType, nAmount);
        } else {
            //But if an array of mana was specified, then use that to add mana
            plyrTarget.mana.AddMana(arnAmount);
        }

        //TODO:: Maybe add a flag for if there should be a timer shown or not
        fDelay = 0.0f;
        sLabel = "Changing mana for player " + plyrTarget.id;

        base.ExecuteEffect();
    }



}
