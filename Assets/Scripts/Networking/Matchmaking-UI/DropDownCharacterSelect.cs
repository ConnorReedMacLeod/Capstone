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

        Debug.Assert(0 <= idChr && idChr < Match.NINITIALCHRSPERTEAM);

        CharType.CHARTYPE chartypeSelected = (CharType.CHARTYPE)dropdown.value;

        NetworkMatchSetup.SetCharacterOrdering(plyrselectorParent.idPlayer, idChr, chartypeSelected);

        LoadoutManager.Loadout loadoutStarting = LoadoutManager.LoadSavedLoadoutForChr(chartypeSelected, 0);

        //Now that our character for this ordering slot has been provided, we need to load in a starting loadout for that character
        NetworkMatchSetup.SetLoadout(plyrselectorParent.idPlayer, idChr, loadoutStarting);

        Debug.LogFormat("Changed chr to {0} with a starting loadout of {1}", chartypeSelected, loadoutStarting);

    }

    public void OnClickEditLoadout() {
        plyrselectorParent.EditChrLoadout(idChr);
    }
}
