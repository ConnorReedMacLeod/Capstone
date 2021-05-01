using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftableChrCollection : MonoBehaviour {


    public DraftableChr[] arDraftableChrPortraits;

    public void UpdateDraftableCharacterPortraits(bool[] arbDraftableChrs) {

        for(int i = 0; i < arbDraftableChrs.Length; i++) {
            arDraftableChrPortraits[i].SetChrInSlot((Chr.CHARTYPE)i);
        }

    }

    public void SetChrAsDrafted(int iChrSlot) {

        arDraftableChrPortraits[iChrSlot].GreyOut();

    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
