using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Stores information needed for playing a randomized sound effect
public class SoundEffect {

    public string sPath;
    public float fDelayBefore;
    public float fDelayExecutable;
    public float fDuration;


    public SoundEffect(string _sPath, float _fDuration, float _fDelayExecutable, float _fDelayBefore = 0) {
        sPath = _sPath;
        fDuration = _fDuration;
        fDelayExecutable = _fDelayBefore;
        fDelayBefore = _fDelayBefore;
    }

    public SoundEffect(string _sPath, float _fDuration, float _fDelayBefore = 0) : this(_sPath, _fDuration, _fDuration, _fDelayBefore) { }

}
