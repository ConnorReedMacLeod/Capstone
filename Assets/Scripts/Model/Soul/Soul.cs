﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO probably extend this class for visible/locked/duration interactions rather than using bool flags
public class Soul : MonoBehaviour {

    public string sName;

    public bool bVisible;     //Is the effect visible in the soul? (can be interacted with)
    public bool bLocked;      //Should the effect not be able to be pushed out of the soul be new effects

    public int nMaxStacks;
    public int nCurStacks;

    public bool bDuration; 
    public int nMaxDuration;
    public int nCurDuration;

    public System.Action funcOnApplication;
    public System.Action funcOnRemoval;

    public void OnApply() {
        if(bDuration == true) {
            nCurDuration = nMaxDuration;
        }

        funcOnApplication();


    }

	
}