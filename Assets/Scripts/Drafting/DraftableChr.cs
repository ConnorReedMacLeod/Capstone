using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftableChr : MonoBehaviour {

    public Chr.CHARTYPE chrInSlot;





    public void SetChrInSlot(Chr.CHARTYPE _chrInSlot) {
        chrInSlot = _chrInSlot;

        string sChrName = Chr.ARSCHRNAMES[(int)chrInSlot];

        LibView.AssignSpritePathToObject("Images/Chrs/" + sChrName + "/img" + sChrName + "neutral", this.gameObject);
    }

    // Update is called once per frame
    void Update() {

    }
}
