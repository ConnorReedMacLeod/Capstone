using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ViewSkillSelection : Singleton<ViewSkillSelection> {

    public Dropdown dropdownSkillSelection;



    public void SetDropDownOptions(Chr chrSelectingSkill) {
        //Clear out the current list of options
        dropdownSkillSelection.ClearOptions();

        List<Dropdown.OptionData> lstNewOptions;

        //Query which skills are selectable with the character's disciplines and convert the names of those skills to dropdowndata items
        lstNewOptions = SkillType.GetsSkillsUnderDisciplines(chrSelectingSkill.lstDisciplines).Select(info => new Dropdown.OptionData(info.sName)).ToList();

        dropdownSkillSelection.AddOptions(lstNewOptions);

    }

    public void ShowSkillSelectionForChr(Chr chrSelectingSkill) {
        this.gameObject.SetActive(true);
        SetDropDownOptions(chrSelectingSkill);
    }

    public void HideSkillSelection() {
        this.gameObject.SetActive(false);
    }

    public override void Init() {

    }
}
