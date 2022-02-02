using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DropDownCharacterSelect : MonoBehaviour {


    public PlayerSelector plyrselectorParent;
    public Dropdown dropdown;
    public int idChr;

    public void UpdateDropdownOptions() {
        
        LibView.SetDropdownOptions(dropdown, CharType.GetAllChrNames());

        //Ensure the default-selected option for this dropdown is mirroring the default in the matchsetup
        dropdown.SetValueWithoutNotify((int)NetworkMatchSetup.GetCharacterSelection(plyrselectorParent.idPlayer, idChr));

        dropdown.RefreshShownValue();
    }

    public void OnChrSelectChange() {

        Debug.Assert(0 <= idChr && idChr < 3);

        NetworkMatchSetup.SetCharacterSelection(plyrselectorParent.idPlayer, idChr, (CharType.CHARTYPE)dropdown.value);

        //Now that our character has been reselected, we need to load in a starting loadout for that character
        NetworkMatchSetup.SetLoadout(plyrselectorParent.idPlayer, idChr,
            LoadoutManager.LoadSavedLoadoutForChr(NetworkMatchSetup.GetCharacterSelection(plyrselectorParent.idPlayer, idChr), 0));

    }

    public void OnClickEditLoadout() {
        plyrselectorParent.EditChrLoadout(idChr);
    }
}
