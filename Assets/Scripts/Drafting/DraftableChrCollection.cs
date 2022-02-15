using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftableChrCollection : MonoBehaviour {


    public DraftableChrDisplay[] arDraftableChrDisplays;

    public GameObject pfDraftableChr;

    public GameObject goContent;

    public void InitDraftableCharacterPortraits() {
        //For each possible character in the game that can be drafted, spawn an icon for it
        // and add it to our array of ChrPortraits

        arDraftableChrDisplays = new DraftableChrDisplay[(int)CharType.CHARTYPE.LENGTH];

        for(int i = 0; i < (int)CharType.CHARTYPE.LENGTH; i++) {
            //Spawn a new Icon for this character
            GameObject goDraftableChr = Instantiate(pfDraftableChr, goContent.transform) as GameObject;

            //Save a reference to its DraftableChr component
            arDraftableChrDisplays[i] = goDraftableChr.GetComponent<DraftableChrDisplay>();

            //Let it know which character it will be representing
            arDraftableChrDisplays[i].SetChrInSlot((CharType.CHARTYPE)i);

            //arDraftableChrPortraits[i].RedOut();
        }

    }

    public void SetChrAsDrafted(int iChrSlot) {

        arDraftableChrDisplays[iChrSlot].GreyOut();

    }

    public void SetChrAsBanned(int iChrSlot) {

        arDraftableChrDisplays[iChrSlot].RedOut();

    }

    // Start is called before the first frame update
    void Start() {
        InitDraftableCharacterPortraits();
    }

    // Update is called once per frame
    void Update() {

    }
}
