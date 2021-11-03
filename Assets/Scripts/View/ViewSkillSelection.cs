using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ViewSkillSelection : Singleton<ViewSkillSelection> {

    public Dropdown dropdownSkillSelection;

    public void SetDropDownOptions(List<SkillType.SkillTypeInfo> lstSkillTypeInfo) {
        //Clear out the current list of options
        dropdownSkillSelection.ClearOptions();

        List<Dropdown.OptionData> lstNewOptions;

        lstNewOptions = lstSkillTypeInfo.Select(info => new Dropdown.OptionData(info.sName)).ToList();

        dropdownSkillSelection.AddOptions(lstNewOptions);

    }

    public void ShowSkillSelectionForChr(Chr chrSelectingSkill) {
        this.gameObject.SetActive(true);
        SetDropDownOptions(SkillType.GetSkillInfosUnderDisciplines(chrSelectingSkill));
    }

    public void HideSkillSelection() {
        this.gameObject.SetActive(false);
    }

    public override void Init() {

    }
}
