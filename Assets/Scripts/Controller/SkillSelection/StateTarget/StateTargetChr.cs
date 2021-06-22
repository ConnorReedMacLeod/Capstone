using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used for targgeting a specific character
public class StateTargetChr : StateTarget {

    public static Subject subAllStartSelection = new Subject(Subject.SubType.ALL);
    public static Subject subAllFinishSelection = new Subject(Subject.SubType.ALL);

    public void cbCancelTargetting(object target, params object[] args) {
        ContLocalUIInteraction.Get().CancelTar();
    }

    public void cbTargetChr(object target, params object[] args) {

        //We clicked on a character, so let's make a SelectionInfo package for it
        SelectionSerializer.SelectionChr infoSelectionChr =
            new SelectionSerializer.SelectionChr(
                ContLocalUIInteraction.Get().chrSelected,
                ContLocalUIInteraction.Get().skillSelected,
                ((ViewChr)target).mod);

        if(infoSelectionChr.CanSelect()) {

            ContLocalUIInteraction.Get().FinishTargetting(infoSelectionChr);

        } else {
            Debug.Log(((ViewChr)target).mod.sName + ", on team " + ((ViewChr)target).mod.plyrOwner.id + " is not a valid character target");
        }
    }

    public void cbSwitchSkill(Object target, params object[] args) {

        Debug.Log("attempting to reselect" + ((ViewSkill)target).mod.sDisplayName);

        ContLocalUIInteraction.Get().StartTargetting(((ViewSkill)target).mod);

    }

    public void NotifySelectableTargets() {

        Skill skillTargetting = ContLocalUIInteraction.Get().skillSelected;

        foreach(Chr chrPossibleTarget in ((ClauseChr)skillTargetting.GetDominantClause()).GetSelectable()) {
            //Let each targettable character know that they are targettable by this currently selected skill - can highlight them 
            chrPossibleTarget.subBecomesTargettable.NotifyObs(null, skillTargetting);
        }
    }

    public void NotifySelectableTargetsEnded() {

        Skill skillTargetting = ContLocalUIInteraction.Get().skillSelected;

        foreach(Chr chrPossibleTarget in ((ClauseChr)skillTargetting.GetDominantClause()).GetSelectable()) {
            //Let each targettable character know that this skill is done targetting, so we can clear anything out that was done when they were targettable
            chrPossibleTarget.subEndsTargettable.NotifyObs(null, skillTargetting);
        }
    }

    override public void OnEnter() {

        Debug.Assert(ContLocalUIInteraction.Get().chrSelected != null);

        Debug.Log("chrSelected is " + ContLocalUIInteraction.Get().chrSelected.sName);

        Arena.Get().view.subMouseClick.Subscribe(cbCancelTargetting);
        ViewInteractive.subGlobalMouseRightClick.Subscribe(cbCancelTargetting);

        ViewChr.subAllClick.Subscribe(cbTargetChr);
        ViewSkill.subAllClick.Subscribe(cbSwitchSkill);
        ViewRestButton.subAllClick.Subscribe(cbSwitchSkill);

        ContLocalUIInteraction.subAllStartManualTargetting.NotifyObs();
        NotifySelectableTargets();
    }

    override public void OnLeave() {

        Arena.Get().view.subMouseClick.UnSubscribe(cbCancelTargetting);
        ViewInteractive.subGlobalMouseRightClick.UnSubscribe(cbCancelTargetting);

        ViewChr.subAllClick.UnSubscribe(cbTargetChr);
        ViewSkill.subAllClick.UnSubscribe(cbSwitchSkill);

        NotifySelectableTargetsEnded();
    }

}
