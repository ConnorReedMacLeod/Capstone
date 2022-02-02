using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelector : MonoBehaviour {

    public int idPlayer;

    public GameObject pfLoadoutSelector;
    public LoadoutSelector loadoutselectActive;
    public Dropdown dropdownOwner;
    public Dropdown dropdownInput;
    public DropDownCharacterSelect[] arDropdownCharSelect;

    public List<LoadoutManager.Loadout> lstLoadoutSelected;

    public static CharType.CHARTYPE[,] CHRSELECTIONSDEFAULT = 
        {{CharType.CHARTYPE.FISCHER, CharType.CHARTYPE.KATARINA, CharType.CHARTYPE.PITBEAST },
        {CharType.CHARTYPE.RAYNE, CharType.CHARTYPE.SAIKO, CharType.CHARTYPE.SOPHIDIA }};

    public void Start() {
        lstLoadoutSelected = new List<LoadoutManager.Loadout>();

        //Initially save the selected loadouts as just being the default loadout for the default character in that position
        for (int i = 0; i < arDropdownCharSelect.Length; i++) {

            //Initially set the selected char to the default for that player+slot combo
            NetworkMatchSetup.SetCharacterSelection(idPlayer, i, CHRSELECTIONSDEFAULT[idPlayer, i]);

            //Set the loadout to be the default loadout for the selected player
            lstLoadoutSelected.Add(LoadoutManager.GetDefaultLoadoutForChar((CharType.CHARTYPE)arDropdownCharSelect[i].dropdown.value));

            //Ensure our character selection dropdown is initialized
            arDropdownCharSelect[i].UpdateDropdownOptions();
        }
    }

    public void EditChrLoadout(int iChrToEdit) {
        if(loadoutselectActive != null) {
            Debug.LogError("Can't edit another loadout, since we're already editing one");
            return;
        }

        //Spawn the loadout selector
        loadoutselectActive = GameObject.Instantiate(pfLoadoutSelector, this.transform.parent).GetComponent<LoadoutSelector>();

        loadoutselectActive.BeginSelection(idPlayer, iChrToEdit, CleanupEditLoadout, lstLoadoutSelected[iChrToEdit]);
    }

    public void CleanupEditLoadout() {
        //Save the generated loadout in the slot for the character we were editing
        lstLoadoutSelected[loadoutselectActive.iChrSelectingFor] = loadoutselectActive.loadoutCur;

        //Once the loadout editing is done, we can just destroy the loadout selector window, and clear out our reference to it
        GameObject.Destroy(loadoutselectActive.gameObject);
        loadoutselectActive = null;
    }

    //If we are indeed using these 'direct to match' inputs to set up the match, then we'll need to save the character selections as though
    //  we went through a draft
    public void PublishCharacterSelections() {
        for(int i=0; i< arDropdownCharSelect.Length; i++) {
            NetworkMatchSetup.SetCharacterSelection(idPlayer, i, (CharType.CHARTYPE)arDropdownCharSelect[i].dropdown.value);
        }
    }

    //If we are indeed using these 'direct to match' inputs to set up the match, then we'll need to save the character selections as though
    //  we went through a draft
    public void PublishLoadouts() {
        for (int i = 0; i < arDropdownCharSelect.Length; i++) {
            //Send over whatever loadout we have locally stored
            NetworkMatchSetup.SetLoadout(idPlayer, i, lstLoadoutSelected[i]);
        }
    }

}
