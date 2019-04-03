using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public AudioSource srcEffect;                   //A reference to the audiosource that we'll play
    public static AudioManager inst = null;     //singleton pattern reference           
    public float lowPitchRange = .95f;              //The lowest a sound effect will be randomly pitched
    public float highPitchRange = 1.05f;            //The highest a sound effect will be randomly pitched


    public static AudioManager Get() {
        return inst;
    }

    void Awake() {
        //Check if there is already an instance of SoundManager
        if (inst == null)
            //if not, set it to this.
            inst = this;
        //If instance already exists:
        else if (inst != this)
            //Destroy this, which enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy(gameObject);

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
    }


    //Used to play single sound clips.
    public float PlaySingle(AudioClip clip) {
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        srcEffect.clip = clip;

        //Play the clip.
        srcEffect.Play();

        return clip.length;
    }


    //RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
    public float PlaySoundEffect(SoundEffect[] arSoundEffects) {

        //If there is no sound effects attached to this, then no need to play anything
        if (arSoundEffects.Length == 0) return 0;

        //Generate a random number between 0 and the length of our array of clips passed in.
        int randomIndex = Random.Range(0, arSoundEffects.Length);

        //Choose a random pitch to play back our clip at between our high and low pitch ranges.
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        //Set the pitch of the audio source to the randomly chosen pitch.
        srcEffect.pitch = randomPitch;

        //Load the sound effect that's been passed to us
        AudioClip clip = Resources.Load("Sounds/" + arSoundEffects[randomIndex].sPath) as AudioClip;

        Debug.Assert(clip != null, "Sounds/" + arSoundEffects[randomIndex].sPath + " could not be loaded");

        //Play the selected clip and return the time of that clip
        return PlaySingle(clip);
    }
}