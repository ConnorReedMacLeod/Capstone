using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPosition : ViewInteractive {

    public Position mod;

    public enum SelectabilityState {
        NONE, ALLYSELECTABLE, ENEMYSELECTABLE
    };

    public static Subject subAllClick = new Subject(Subject.SubType.ALL);

    public void UpdateChrOnPositionToHere(Object target, params object[] args) {

        if(mod.chrOnPosition == null) return;

        //Move their global position to our global position
        mod.chrOnPosition.view.transform.position = this.transform.position;

    }


    //For when the currently targetting skill can target this position
    public void cbOnBecomesTargettable(Object target, params object[] args) {
        Skill skillTargetting = (Skill)args[0];

        Debug.Log(mod + " is currently targettable by " + skillTargetting.sName);

        //If the source of this skill was an ally
        if (mod.IsAllyOwned(skillTargetting.chrOwner.plyrOwner)) {
            DecideIfHighlighted(SelectabilityState.ALLYSELECTABLE);
        } else {
            DecideIfHighlighted(SelectabilityState.ENEMYSELECTABLE);
        }

    }

    //For when the currently targetting skill has stopped, so this character can clear any targetting display
    public void cbOnEndsTargettable(Object target, params object[] args) {
        Debug.Log(mod + " is no longer targettable");
        DecideIfHighlighted(SelectabilityState.NONE);
    }

    public void DecideIfHighlighted(SelectabilityState selectState) {
        string sSprPath = "Images/Chrs/imgGlow";

        switch (selectState) {
            case SelectabilityState.NONE:
                //No additional suffix needed for the path
                break;
            case SelectabilityState.ALLYSELECTABLE:
                sSprPath += "4";
                break;
            case SelectabilityState.ENEMYSELECTABLE:
                sSprPath += "6";
                break;
            default:
                Debug.Log("Unrecognized SelectabilityState: " + selectState);
                break;
        }

        LibView.AssignSpritePathToObject(sSprPath, this.gameObject);
    }

    public override void Start() {
        base.Start();

        mod.Start();

        mod.subChrEnteredPosition.Subscribe(UpdateChrOnPositionToHere);


        mod.subBecomesTargettable.Subscribe(cbOnBecomesTargettable);
        mod.subEndsTargettable.Subscribe(cbOnEndsTargettable);
    }
}
