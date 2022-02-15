using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Executable {

    public Chr chrSource;

    public string sLabel;
    public float fDelay;

    public bool bPreTriggered;

    public bool bCancelIfSourceDies;

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

    public IEnumerator Execute() {
        if (isLegal() == false) {
            Debug.Log("Executable of type  " + this.GetType().ToString() + " has been cancelled since it's no longer legal");
            yield break;
        }

        //If the executable  is legal, then do its effect

        //Perform all of the effects of the executable
        ExecuteEffect();

        //Put our post-trigger effects onto the stack so they'll be executed next
        GetPostTrigger().NotifyObs(null, this);

        //Can extend either of these as needed
        PlaySoundEffects();
        yield return PlayAnimations(); 
    }

    public abstract void ExecuteEffect();

    public virtual void PlaySoundEffects() {
        //Let the AudioManager play the associated sound effect (if there is one)
        if (arSoundEffects != null && arSoundEffects.Length != 0) {
            float fPlayTime = AudioManager.Get().PlaySoundEffect(arSoundEffects);
            //Debug.Log("fPlayTime is " + fPlayTime + " for " + this.sLabel);
        }
    }

    public virtual IEnumerator PlayAnimations() {
        if (fDelay == 0f) yield break;

        //By default, we'll just start a timer with an appropriate label, then wait for it to finish

        ContSkillEngine.Get().SpawnTimer(fDelay, sLabel);

        yield return new WaitForSeconds(fDelay);
    }

    public Executable(Chr _chrSource) {
        chrSource = _chrSource;
        fDelay = ContTime.fDelayStandard;
    }

    public Executable(Executable other) {
        
        chrSource = other.chrSource;
        sLabel = other.sLabel;
        fDelay = other.fDelay;
        bPreTriggered = other.bPreTriggered;
        bCancelIfSourceDies = other.bCancelIfSourceDies;

        arSoundEffects = other.arSoundEffects;

    }

}
