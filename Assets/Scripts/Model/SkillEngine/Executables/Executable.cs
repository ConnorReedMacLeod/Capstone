﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Executable {

    public Chr chrSource;

    public string sLabel;
    public float fDelay;

    public bool bPreTriggered;

    public bool bCancelIfSourceDies;

    public bool bStopAutoProcessing;

    public SoundEffect[] arSoundEffects;

    public abstract Subject GetPreTrigger();
    public abstract Subject GetPostTrigger();
    public abstract List<Replacement> GetReplacements();
    public abstract List<Replacement> GetFullReplacements();

    public virtual bool isLegal() {
        if (bCancelIfSourceDies && chrSource != null && chrSource.bDead) {
            Debug.Log("Executable of type  " + this.GetType().ToString() + " not legal since " + chrSource.sName + "(source) is dead");
            return false;
        }
        return true;
    }

    public void Execute() {
        if (isLegal() == false) {
            Debug.Log("Executable of type  " + this.GetType().ToString() + " has been cancelled since it's no longer legal");

        } else {
            //If the executable  is legal, then do its effect

            //Perform all of the effects of the executable
            ExecuteEffect();


            //Let the AudioManager play the associated sound effect (if there is one)
            if (arSoundEffects != null && arSoundEffects.Length != 0) {
                float fPlayTime = AudioManager.Get().PlaySoundEffect(arSoundEffects);
                //Debug.Log("fPlayTime is " + fPlayTime + " for " + this.sLabel);
            }

            //Put our post-trigger effects onto the stack so they'll be executed next
            GetPostTrigger().NotifyObs(null, this);

        }

        //Now that we've done our thing, let the engine know to start processing the next thing
        ContSkillEngine.Get().InvokeProcessStack(fDelay, sLabel, bStopAutoProcessing);
    }

    public abstract void ExecuteEffect();

    public Executable(Chr _chrSource) {
        chrSource = _chrSource;
        fDelay = ContTurns.fDelayStandard;
    }

    public Executable(Executable other) {
        
        chrSource = other.chrSource;
        sLabel = other.sLabel;
        fDelay = other.fDelay;
        bPreTriggered = other.bPreTriggered;
        bStopAutoProcessing = other.bStopAutoProcessing;
        bCancelIfSourceDies = other.bCancelIfSourceDies;

        arSoundEffects = other.arSoundEffects;

    }

}
