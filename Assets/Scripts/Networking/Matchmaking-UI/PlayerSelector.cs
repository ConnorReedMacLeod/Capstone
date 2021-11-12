using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelector : MonoBehaviour {

    public int idPlayer;

    public GameObject pfLoadoutSelector;
    public LoadoutSelector loadoutselectActive;

    public void EditChrLoadout(int iChrToEdit) {
        if(loadoutselectActive != null) {
            Debug.LogError("Can't edit another loadout, since we're already editing one");
            return;
        }

        //Spawn the loadout selector
        loadoutselectActive = GameObject.Instantiate(pfLoadoutSelector, this.transform.parent).GetComponent<LoadoutSelector>();

        loadoutselectActive.BeginSelection(idPlayer, iChrToEdit, CleanupEditLoadout);
    }

    public void CleanupEditLoadout() {
        //Once the loadout editing is done, we can just destroy the loadout selector window, and clear out our reference to it
        GameObject.Destroy(loadoutselectActive.gameObject);
        loadoutselectActive = null;
    }
}
