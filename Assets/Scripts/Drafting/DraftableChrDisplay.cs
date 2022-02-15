using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraftableChrDisplay : DraftChrDisplay {

    public void onClick() {

        Debug.Log("Clicked on " + chrInSlot);

        DraftController.Get().OnDraftableChrClicked(chrInSlot);
            
    }


}
