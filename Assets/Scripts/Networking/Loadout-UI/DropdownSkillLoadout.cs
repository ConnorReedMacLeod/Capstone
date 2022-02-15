using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownSkillLoadout : MonoBehaviour {

    public int iSkillSlot;
    public Dropdown dropdownSkillLoadout;

    public LoadoutSelector loadoutselector;

    

    public void Init(SkillType.SKILLTYPE skilltypeSelected) {

        LibView.SetSkillTypeDropDownOptions(dropdownSkillLoadout, loadoutselector.lstSelectableSkills);

        SetDropdownSelection(skilltypeSelected);
    }

    public void SetDropdownSelection(SkillType.SKILLTYPE skilltypeSelected) {
        bool bFoundSelected = false;

        //Find the index for the selected skilltype we're supposed to be initially set to
        for (int i = 0; i < loadoutselector.lstSelectableSkills.Count; i++) {
            if (loadoutselector.lstSelectableSkills[i].type == skilltypeSelected) {
                //Make sure not to re-notify that the value has changed, since this will infinitely loop
                dropdownSkillLoadout.SetValueWithoutNotify(i);

                dropdownSkillLoadout.RefreshShownValue();

                bFoundSelected = true;
                break;
            }
        }

        if (bFoundSelected == false) {
            Debug.LogError("Attempted to initially select " + skilltypeSelected + " but this isn't in the lst of skills we can select from");
        }
    }

    public void OnSelectedChange() {

        //Pass along the newly chosen skill to the loadoutselector
        loadoutselector.LoadoutSkillChanged(iSkillSlot, loadoutselector.lstSelectableSkills[dropdownSkillLoadout.value].type);
    }



}
