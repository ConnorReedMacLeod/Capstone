using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All this really needs to do is add a chrTarget field
public class SoulChr : Soul {

    public Chr chrTarget;     //A reference to the character this soul effect is applied to
    public Position posOriginallyAppliedOn; //A saved reference to the position the character was on when it was originally applied


    public SoulChr(Chr _chrSource, Chr _chrTarget, Skill _skillSource) : base(_chrSource, _skillSource) {

        chrTarget = _chrTarget;
        if(chrTarget != null) posOriginallyAppliedOn = chrTarget.position;
    }

    public SoulChr(SoulChr soulToCopy, Chr _chrTarget = null) : base(soulToCopy) {

        if(_chrTarget != null) {
            //If a Target was provided, then we'll use that
            chrTarget = _chrTarget;
        } else {
            //Otherwise, just copy from the other object
            chrTarget = soulToCopy.chrTarget;
        }
        if(chrTarget != null) posOriginallyAppliedOn = chrTarget.position;

    }

    public override string GetNameOfAppliedTo() {
        return chrTarget.sName;
    }

    //Can subscribe with this if you want to remove the effect on a particular trigger (doesn't have any extra checks though)
    public void cbRemoveThis(Object target, params object[] args) {
        Debug.Log("Pushing executable to remove " + this.sName + " from " + chrTarget.sName + " since it's now on " + chrTarget.position.ToString());
        ContSkillEngine.PushSingleExecutable(new ExecRemoveSoulChr(chrSource, this));

        Debug.Log("Need to unsubscribe all active subscriptions since we're being removed");
    }

    //Can initially subscribe to the character's OnChrLeftPosition trigger if you want this effect to only apply while they stay on their original position
    public void cbRemoveIfPositionChanged(Object target, params object[] args) {
        if(chrTarget.position != posOriginallyAppliedOn) {
            Debug.Log("Since " + chrTarget.sName + " has moved away from " + posOriginallyAppliedOn + ", then we'll remove " + sName);
            cbRemoveThis(target, args);
        }
    }

}
