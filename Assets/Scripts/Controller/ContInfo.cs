using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContInfo : MonoBehaviour {

    bool bStarted;

    public enum StateInfo { SKILL };

    public StateInfo stateInfo; //TODO:: Alternate which subscriptions you are using depending on
                                //       which state you're currently in
    public bool bLocked; //TODO:: Flesh out a target locking system more

    public ViewInfoPanel viewInfoPanel;
    public Skill skillFocus;

    public void cbStartTargetting(Object target, params object[] args) {
        SetSkillFocus(ContLocalUIInteraction.Get().selectionsInProgress.skillslotSelected.skill);
        bLocked = true;
    }

    public void cbFinishTargetting(Object target, params object[] args) {
        ClearSkillFocus();
        bLocked = false;
    }

    public void DisplaySkill(Skill skill) {
        if(bLocked == false) {
            viewInfoPanel.ShowInfoSkill(skill);
        }
    }

    public void cbSoulStartHover(Object target, params object[] args) {
        if(((ViewSoul)target).mod == null || ((ViewSoul)target).mod.skillSource == null) {
            //Debug.Log("No skill source to display");
        } else {
            //Debug.Log("Displaying " + ((ViewSoul)target).mod.sName);
            DisplaySkill(((ViewSoul)target).mod.skillSource);
        }
    }

    public void cbSkillStartHover(Object target, params object[] args) {

        ViewSkill viewskillHovered = (ViewSkill)target;

        if (viewskillHovered.mod == null) {
            Debug.Log("Hovering a null skill - no need to display any info for this");
            return;
        }

        DisplaySkill(viewskillHovered.mod);
    }

    public void cbRestButtonStartHover(Object target, params object[] args) {
        DisplaySkill(ContTurns.Get().GetNextActingChr().arSkillSlots[Chr.iRestSkill].skill);
    }

    public void StopDisplaySkill(Skill sk) {
        if(bLocked == false &&
            ((viewInfoPanel.viewInfoSkill == null) || //If nothing is currently being shown
            sk == viewInfoPanel.viewInfoSkill.mod)) {
            // First ensure that what we're leaving is the current displayed skill
            //When we stop hovering over the thing we're displaying, stop displaying it
            viewInfoPanel.ClearPanel();
        }
    }

    public void cbSoulStopHover(Object target, params object[] args) {
        if(((ViewSoul)target).mod == null) {
            StopDisplaySkill(null);
        } else {
            StopDisplaySkill(((ViewSoul)target).mod.skillSource);
        }
    }

    public void cbSkillStopHover(Object target, params object[] args) {

        ViewSkill viewskillHovered = (ViewSkill)target;

        if (viewskillHovered.mod == null) {
            Debug.Log("Hovering a null skill - no need react to anything for this");
            return;
        }

        StopDisplaySkill(viewskillHovered.mod);
    }

    public void cbRestButtonStopHover(Object target, params object[] args) {
        StopDisplaySkill(ContTurns.Get().GetNextActingChr().arSkillSlots[Chr.iRestSkill].skill);
    }

    public void SetSkillFocus(Skill _skillFocus) {
        viewInfoPanel.ClearPanel();
        skillFocus = _skillFocus;
        viewInfoPanel.ShowInfoSkill(skillFocus);
    }

    public void ClearSkillFocus() {
        skillFocus = null;
        viewInfoPanel.ClearPanel();//TODO:: This feels like it could maybe bug
    }

    public void Start() {
        if(!bStarted) {
            bStarted = true;

            if(viewInfoPanel == null) {
                Debug.LogError("ERROR! VIEWINFOPANEL REFERENCE NOT SET!");
            }

            ContLocalUIInteraction.subAllStartManualSelections.Subscribe(cbStartTargetting);
            ContLocalUIInteraction.subAllFinishManualSelections.Subscribe(cbFinishTargetting);

            ViewSkill.subAllStartHover.Subscribe(cbSkillStartHover);
            ViewSkill.subAllStopHover.Subscribe(cbSkillStopHover);

            ViewSoul.subAllStartHover.Subscribe(cbSoulStartHover);
            ViewSoul.subAllStopHover.Subscribe(cbSoulStopHover);
            ViewRestButton.subAllStartHover.Subscribe(cbRestButtonStartHover);
            ViewRestButton.subAllStopHover.Subscribe(cbRestButtonStopHover);
        }
    }
}
