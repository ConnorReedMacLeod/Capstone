using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewSoulContainer : MonoBehaviour {

    public SoulContainer mod;
    
    public GameObject[] argoSoul;



    //Sets the sprite used for the ith soul entry
    public void SetSoulSprite(int i) {

        string sSprPath = "Images/Soul/imgSoulEmpty";

        if (mod.arSoul[i] != null) {

            sSprPath = "Images/Soul/imgSoul" + mod.arSoul[i].sName;

        }
        Sprite sprSoul = Resources.Load(sSprPath, typeof(Sprite)) as Sprite;

        argoSoul[i].GetComponent<SpriteRenderer>().sprite = sprSoul;

    }


    //Update all display information for each soul icon
    public void cbUpdateSoulSprites(Object target, params object[] args) {

        for (int i = 0; i < argoSoul.Length; i++) {
            SetSoulSprite(i);
        }

    }

    public void SetNumSoulSlots(int _nNumSoulSlots) {
        //TODO:: Account for having a variable number of soul slots

    }


    void Init () {

        SetNumSoulSlots(mod.arSoul.Length);

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
