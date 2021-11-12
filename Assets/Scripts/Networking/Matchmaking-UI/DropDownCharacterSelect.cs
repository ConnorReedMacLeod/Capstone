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
        
        LibView.SetDropdownOptions(dropdown, CharType.GetAllChrNames());

        //Ensure the default-selected option for this dropdown is mirroring the default in the matchsetup
        dropdown.value = (int)MatchSetup.Get().arLocalChrSelections[plyrselectorParent.idPlayer][idChr];

        dropdown.RefreshShownValue();
    }

    public void OnChrSelectChange() {

        Debug.Assert(0 <= idChr && idChr < 3);

        MatchSetup.Get().arLocalChrSelections[plyrselectorParent.idPlayer][idChr] = (CharType.CHARTYPE)dropdown.value;

        //Now that our character has been reselected, we need to load in a starting loadout for that character
        MatchSetup.Get().arLocalLoadoutSelections[plyrselectorParent.idPlayer][idChr] = 
            LoadoutManager.LoadSavedLoadoutForChr(MatchSetup.Get().arLocalChrSelections[plyrselectorParent.idPlayer][idChr], 0);

    }

    public void OnClickEditLoadout() {
        plyrselectorParent.EditChrLoadout(idChr);
    }
}
