using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All this really needs to do is add a chrTarget field
public class SoulPosition : Soul {

    public Position posTarget;     //A reference to the Position this soul effect is applied to

    public List<SoulChr> lstSoulAppliedToChrOnPosition;

    public Chr chrOnPosition {
        get {
            return posTarget.chrOnPosition;
        }
    }

    public SoulPosition(Chr _chrSource, Position _posTarget, Skill _skillSource) : base(_chrSource, _skillSource) {

        posTarget = _posTarget;

    }

    public SoulPosition(SoulPosition soulToCopy, Position _posTarget = null) : base(soulToCopy) {

        if(_posTarget != null) {
            //If a Target was provided, then we'll use that
            posTarget = _posTarget;
        } else {
            //Otherwise, just copy from the other object
            posTarget = soulToCopy.posTarget;
        }

    }


    public virtual List<SoulChr> GetSoulToApplyToChrOnPosition() {
        //By Default, just return an empty list
        return new List<SoulChr>();
    }

    public override void ApplicationEffect() {

        //Ensure that we're monitoring when the character on our position changes so we can update the soul affect we apply to the active character
        posTarget.subChrEnteredPosition.Subscribe(cbChrEnteredPosition);

        //Immediately simulate an 'ChrEnteredPosition" trigger to initially apply our SoulChr effects
        cbChrEnteredPosition(null);
    }

    public override void RemoveEffect() {

        //When this SoulPosition is removed, we need to remove all of the associated effects that have been placed on the character on this position

        for(int i = 0; i < lstSoulAppliedToChrOnPosition.Count; i++) {
            //If the effect has already been removed for some reason, then just skip it
            if(lstSoulAppliedToChrOnPosition[i].bRemoved == true) {
                Debug.Log(lstSoulAppliedToChrOnPosition[i].sName + " was already removed so doesn't need to be removed again");
                continue;
            }

            Debug.Log("Pushing executable to remove " + lstSoulAppliedToChrOnPosition[i].sName + " from " + chrOnPosition.sName + " on " + posTarget.ToString());
            ContSkillEngine.PushSingleExecutable(new ExecRemoveSoulChr(chrSource, lstSoulAppliedToChrOnPosition[i]));

        }

        //Unsubscribe from monitoring our position's subChrEnteredPosition trigger
        posTarget.subChrEnteredPosition.UnSubscribe(cbChrEnteredPosition);
    }


    public virtual void cbChrEnteredPosition(Object target, params object[] args) {

        if(chrOnPosition != null) {

            //Ask whatever extension of SoulPosition we are to generate a list of Soul effects to apply to the character on this position
            // We then save references 
            lstSoulAppliedToChrOnPosition = GetSoulToApplyToChrOnPosition();

            for(int i = 0; i < lstSoulAppliedToChrOnPosition.Count; i++) {
                Debug.Log("Adding OnChrLeftPosition trigger to remove the " + i + "th SoulChr effect for " + this.sName);
                chrOnPosition.subLeftPosition.Subscribe(lstSoulAppliedToChrOnPosition[i].cbRemoveIfPositionChanged);

                Debug.Log("Pushing Executable for adding " + lstSoulAppliedToChrOnPosition[i].sName + " to " + chrOnPosition.sName + " on " + posTarget.ToString());
                ContSkillEngine.PushSingleExecutable(new ExecApplySoulChr(chrSource, chrOnPosition, lstSoulAppliedToChrOnPosition[i]));
            }


        } else {
            Debug.Log("Tried to add a soul effect to the character on " + posTarget.ToString() + ", but no character moved onto " + posTarget.ToString());
        }
    }



    public override string GetNameOfAppliedTo() {
        return posTarget.ToString();
    }
}