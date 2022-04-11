using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


//Note that this dropdown is intended to be used to quickly setup which characters are to be used in a match
//  and in what order they should act.  This accomplishes both tasks by assigning the chosen character to be in
//  the 'idChr'th slot of the character ordering
public class DropDownCharacterSelect : MonoBehaviour {


    public PlayerSelector plyrselectorParent;
    public Dropdown dropdown;
    public int idChr;

    public void UpdateDropdownOptions(CharType.CHARTYPE chartypeInitialSelection) {

        LibView.SetDropdownOptions(dropdown, CharType.GetAllChrNames());

        //Ensure the default-selected option for this dropdown is mirroring the default in the matchsetup
        dropdown.SetValueWithoutNotify((int)chartypeInitialSelection);

        dropdown.RefreshShownValue();
    }

    public void OnCharacterOrderingChange() {

        Debug.Assert(0 <= idChr && idChr < 3);

        NetworkMatchSetup.SetCharacterOrdering(plyrselectorParent.idPlayer, idChr, (CharType.CHARTYPE)dropdown.value);

        //Now that our character for this ordering slot has been provided, we need to load in a starting loadout for that character
        NetworkMatchSetup.SetLoadout(plyrselectorParent.idPlayer, idChr,
            LoadoutManager.LoadSavedLoadoutForChr(NetworkMatchSetup.GetCharacterOrdering(plyrselectorParent.idPlayer, idChr), 0));

        Debug.LogFormat("Changed chr to {0} with a starting loadout of {1}", NetworkMatchSetup.GetCharacterOrdering(plyrselectorParent.idPlayer, idChr),
            NetworkMatchSetup.GetLoadout(plyrselectorParent.idPlayer, idChr));

    }

    public void OnClickEditLoadout() {
        plyrselectorParent.EditChrLoadout(idChr);
    }
}
