using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutSelector : MonoBehaviour {

    public Text txtLabel;
    public InputField inputfieldLoadoutName;

    public DropdownSkillLoadout[] ardropdownSkillLoadout = new DropdownSkillLoadout[Chr.nTotalCharacterSkills];
    
    public LoadoutManager.Loadout loadoutCur;
    public Dropdown dropdownSavedLoadouts;

    public int iPlayerSelectingFor;
    public int iChrSelectingFor;

    public List<SkillType.SkillTypeInfo> lstSelectableSkills;

    public CharType.CHARTYPE ChrTypeSelectingFor() {
        return NetworkMatchSetup.GetCharacterSelection(iPlayerSelectingFor, iChrSelectingFor);
    }

    public System.Action fnOnSelectionComplete;

    public void SaveLoadout() {

        //Update the build's name to whatever the inputfield has been entered with
        loadoutCur.sName = inputfieldLoadoutName.text;

        //Overwrite the stored loadout slot with whatever loadout is currently configured
        LoadoutManager.SaveLoadout(ChrTypeSelectingFor(), dropdownSavedLoadouts.value, loadoutCur);

        Debug.Log("Saved " + loadoutCur);

        //Update the LoadoutDropdown entry's name to reflect this newly saved loadout
        dropdownSavedLoadouts.options[dropdownSavedLoadouts.value].text = loadoutCur.sName;

        dropdownSavedLoadouts.RefreshShownValue();
    }

    public void LoadLoadout() {
        //Load whichever loadout slot is currently selected in the loadout dropdown
        InitWithLoadout(LoadoutManager.LoadSavedLoadoutForChr(ChrTypeSelectingFor(), dropdownSavedLoadouts.value));
    }

    public void InitWithLoadout(LoadoutManager.Loadout loadout) {
        //Set our local loadout to the newly submitted one
        loadoutCur = loadout;

        //Setup the general UI to reflect the new loadout
        inputfieldLoadoutName.text = loadoutCur.sName;

        lstSelectableSkills = SkillType.GetSkillInfosUnderDisciplines(ChrTypeSelectingFor());

        //Loop through all the dropdown skill selectors and initialize them with what options they are set to (and their currently selectable options)
        for (int i = 0; i < ardropdownSkillLoadout.Length; i++) {
            //Select the appropriate skill (either equipped or benched) from the current loadout
            ardropdownSkillLoadout[i].Init(loadoutCur.lstChosenSkills[i]);
        }
    }

    public void LoadoutSkillChanged(int iSkillSlot, SkillType.SKILLTYPE skilltypeNew) {
        //Save the previous selection
        SkillType.SKILLTYPE skilltypePrev = loadoutCur.lstChosenSkills[iSkillSlot];

        //Update our loadout to reflect the new skill
        loadoutCur.lstChosenSkills[iSkillSlot] = skilltypeNew;

        //Scan through our loadout skills to see if one of them already is equipped with this skill
        for(int i=0; i<loadoutCur.lstChosenSkills.Count; i++) {
            if (iSkillSlot == i) continue; //Skip over the skillslot that just claimed this skill

            //If one of our skillslots in our loadout already has this skill, instead replace it with the old
            //  skill that we were replacing (effectively swapping the two skills' positions in the loadout)
            if(loadoutCur.lstChosenSkills[i] == skilltypeNew) {
                loadoutCur.lstChosenSkills[i] = skilltypePrev;
                //Update the selection in the dropdown for that skillslot
                ardropdownSkillLoadout[i].SetDropdownSelection(loadoutCur.lstChosenSkills[i]);
            }
        }
    }

    public void BeginSelection(int _iPlayerSelectingFor, int _iChrSelectingFor, System.Action _fnOnSelectionComplete, LoadoutManager.Loadout loadoutToStart) {
        iPlayerSelectingFor = _iPlayerSelectingFor;
        iChrSelectingFor = _iChrSelectingFor;

        fnOnSelectionComplete = _fnOnSelectionComplete;

        txtLabel.text = string.Format("Loadout for {0}", CharType.GetChrName(ChrTypeSelectingFor()));

        //Initialize the saved loadouts dropdown
        InitSavedLoadoutsDropdown();

        //Need to initialize the loadout selector with whatever loadout is currently defined for this character in matchsetup
        InitWithLoadout(loadoutToStart);

    }

    public void InitSavedLoadoutsDropdown() {
        LibView.SetDropdownOptions(dropdownSavedLoadouts, LoadoutManager.LoadAllLoadoutNamesForChr(ChrTypeSelectingFor()));

        dropdownSavedLoadouts.value = 0;

        dropdownSavedLoadouts.RefreshShownValue();

    }

    public void CompleteSelection() {

        Debug.Log("After completing, our loadout is " + loadoutCur);

        //Now that we're done selecting, just callback whatever cleanup method our creator passed to us
        fnOnSelectionComplete();
    }
}
