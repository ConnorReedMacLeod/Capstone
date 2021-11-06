using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftedChrDisplay : MonoBehaviour {

    public DraftableChr[] arDraftedChrSlots;



    public void UpdateDraftedChrDisplay(CharType.CHARTYPE[] arDraftedChrs) {

        for(int i = 0; i < arDraftedChrs.Length; i++) {
            arDraftedChrSlots[i].SetChrInSlot(arDraftedChrs[i]);
        }

    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
