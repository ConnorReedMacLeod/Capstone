using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTargetSelected : StateTarget {

    public void cbDeselect(Object target, params object[] args) {
        ContLocalUIInteraction.Get().SetState(new StateTargetIdle());
    }

    public void cbReselectChar(Object target, params object[] args) {
        // If we now click on a different character, then we'll select them instead
        ContLocalUIInteraction.Get().chrSelected.Idle(); // Need to deselect our current character first
        ContLocalUIInteraction.Get().chrSelected = ((ViewChr)target).mod;

        ContLocalUIInteraction.Get().SetState(new StateTargetSelected());
    }

    public void cbClickSkill(Object target, params object[] args) {
        ViewSkill viewskillClicked = (ViewSkill)target;

        if(viewskillClicked.mod == null) {
            Debug.Log("Clicking a null skill - don't proceed with selection");
            return;
        }

        ContLocalUIInteraction.Get().ChooseSkillToSelect(viewskillClicked.mod);

    }

    public void cbClickRestButton(Object target, params object[] args) {
        Debug.Log("Should make the rest button into a standard action button");
        Skill skillRest = ContLocalUIInteraction.Get().chrSelected.arSkillSlots[Chr.iRestSkill].skill;

        ContLocalUIInteraction.Get().ChooseSkillToSelect(skillRest);

    }


    override public void OnEnter() {
        Debug.Assert(ContLocalUIInteraction.Get().chrSelected != null);

        ViewBackground.subAllBackgroundClick.Subscribe(cbDeselect);
        ViewChr.subAllClick.Subscribe(cbReselectChar);
        ViewSkill.subAllClick.Subscribe(cbClickSkill);
        ViewRestButton.subAllClick.Subscribe(cbClickRestButton);
        KeyBindings.SetBinding(cbClickRestButton, KeyCode.Space);

        ContLocalUIInteraction.Get().chrSelected.Select();

    }

    override public void OnLeave() {

        ViewBackground.subAllBackgroundClick.UnSubscribe(cbDeselect);
        ViewChr.subAllClick.UnSubscribe(cbReselectChar);
        ViewSkill.subAllClick.UnSubscribe(cbClickSkill);
        ViewRestButton.subAllClick.UnSubscribe(cbClickRestButton);
        KeyBindings.Unbind(KeyCode.Space); //clear the binding
    }
}
