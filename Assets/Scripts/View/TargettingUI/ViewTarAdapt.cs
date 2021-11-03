﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ViewTarAdapt : MonoBehaviour {

    public TarAdapt modTarAdapt;
    public List<SkillType.SkillTypeInfo> lstSkillTypeInfosAdaptable;

    public Dropdown dropdownSkillSelection;

    //Set the TarAdapt model that we're going to be facilitating payment for
    public void StartSelection(TarAdapt _modTarAdapt) {

        modTarAdapt = _modTarAdapt;

        //Save a copy of the skills we can adapt into
        lstSkillTypeInfosAdaptable = modTarAdapt.GetAdaptableSkills();

        SetDropDownOptions(lstSkillTypeInfosAdaptable);

        //Set up the submission keybinding
        KeyBindings.SetBinding(SubmitSelectedSkill, KeyCode.T);

    }

    //Clear out anything from the current selection process
    public void CleanUp() {

        //Clear out the submission keybinding
        KeyBindings.Unbind(KeyCode.T);

        lstSkillTypeInfosAdaptable = null;

    }

    public void SetDropDownOptions(List<SkillType.SkillTypeInfo> lstSkillTypeInfo) {
        //Clear out the current list of options
        dropdownSkillSelection.ClearOptions();

        List<Dropdown.OptionData> lstNewOptions;

        lstNewOptions = lstSkillTypeInfo.Select(info => new Dropdown.OptionData(info.sName)).ToList();

        dropdownSkillSelection.AddOptions(lstNewOptions);

    }

    public void OnSelectedChanged() {
        Debug.Log("Dropdown changed to " + dropdownSkillSelection.value);
    }


    public void SubmitSelectedSkill(Object target, params object[] args) {

        //Grab whichever associated Skill we have hovered in the dropdown and pass it as our selection
        SkillType.SKILLTYPE skilltypeSelected = lstSkillTypeInfosAdaptable[dropdownSkillSelection.value].type;

        Debug.Log("Going to pass " + skilltypeSelected + " to adapt into");

        modTarAdapt.AttemptSelection(skilltypeSelected);
    }

}
