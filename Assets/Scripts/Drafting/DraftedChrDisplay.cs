using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftedChrDisplay : MonoBehaviour {

    public DraftChrDisplay[] arDraftedChrDisplays;

    public void UpdateDraftedChrDisplays(CharType.CHARTYPE[] arDraftedChrs) {

        for(int i = 0; i < arDraftedChrs.Length; i++) {

            arDraftedChrDisplays[i].SetChrInSlot(arDraftedChrs[i]);

        }

    }


}
