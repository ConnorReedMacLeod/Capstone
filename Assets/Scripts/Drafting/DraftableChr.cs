﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftableChr : MonoBehaviour {

    public Chr.CHARTYPE chrInSlot;

    public void RedOut() {

        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
    }

    public void GreyOut() {

        gameObject.GetComponent<SpriteRenderer>().color = Color.grey;
    }

    public void SetChrInSlot(Chr.CHARTYPE _chrInSlot) {
        if(chrInSlot == _chrInSlot) {
            return;
        }

        chrInSlot = _chrInSlot;

        string sChrName = Chr.ARSCHRNAMES[(int)chrInSlot];

        LibView.AssignSpritePathToObject("Images/Chrs/" + sChrName + "/img" + sChrName + "neutral", this.gameObject);
    }

    // Update is called once per frame
    void Update() {

    }
}
