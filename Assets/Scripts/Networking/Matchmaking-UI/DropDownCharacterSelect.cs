using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DropDownCharacterSelect : MonoBehaviour {


    public PlayerSelector plyrselectorParent;
    public Dropdown dropdown;
    public int idChr;

    public void Start() {
        //Double check that our CharacterSelection instance has already Start'd itself
        MatchSetup.Get().Start();

        InitChrTypeDropdown();

        //Ensure the default-selected option for this dropdown is mirroring the default in the matchsetup
        dropdown.value = (int)MatchSetup.Get().arLocalChrSelections[plyrselectorParent.idPlayer][idChr];
    }


    public void InitChrTypeDropdown() {
        //Clear out the current list of options
        dropdown.ClearOptions();

        List<Dropdown.OptionData> lstNewOptions;

        lstNewOptions = CharType.dictChrTypeInfos.Values.Select(info => new Dropdown.OptionData(info.sName)).ToList();

        dropdown.AddOptions(lstNewOptions);
    }

    public void OnChrSelectChange(int nChrSelect) {

        Debug.Assert(0 <= idChr && idChr < 3);

        MatchSetup.Get().arLocalChrSelections[plyrselectorParent.idPlayer][idChr] = (CharType.CHARTYPE)nChrSelect;

    }
}
