using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[RequireComponent(typeof(Dropdown))]
public class ViewTarAdapt : Singleton<ViewTarAdapt> {

    public TarAdapt modTarAdapt;
    public List<SkillType.SkillTypeInfo> lstSkillTypeInfosAdaptable;

    public Dropdown dropdownSkillSelection;

    public Vector3 v3OnScreen = new Vector3(0, 0, -2.5f);
    public Vector3 v3OffScreen = new Vector3(-100, -100, -2.5f);

 

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

    public void MoveOnScreen() {
        transform.position = v3OnScreen;
    }

    public void MoveOffscreen() {
        transform.position = v3OffScreen;
    }

    public override void Init() {


    }


    public void SubmitSelectedSkill(Object target, params object[] args) {

        //Grab whichever associated Skill we have hovered in the dropdown and pass it as our selection
        SkillType.SKILLTYPE skilltypeSelected = lstSkillTypeInfosAdaptable[dropdownSkillSelection.value].type;

        modTarAdapt.AttemptSelection(dropdownSkillSelection.value);
    }

}
