using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftController : MonoBehaviour {

    public bool[] arbChrsAvailableToDraft;

    public DraftableChrCollection draftcollection;

    public const int NDRAFTEDCHRSPERPLAYER = 7;

    public Chr.CHARTYPE[][] arDraftedChrs = new Chr.CHARTYPE[2][];
    public int[] arNumChrsDrafted = new int[2];

    public DraftedChrDisplay[] arDraftedChrDisplay = new DraftedChrDisplay[2];

    public void InitAllDraftingPortraits() {

        arbChrsAvailableToDraft = new bool[(int)Chr.CHARTYPE.LENGTH];

        for(int i = 0; i < arbChrsAvailableToDraft.Length; i++) {
            arbChrsAvailableToDraft[i] = true;
        }

        draftcollection.UpdateDraftableCharacterPortraits(arbChrsAvailableToDraft);

    }



    public void DraftChr(int iPlayer, Chr.CHARTYPE chrDrafted) {
        //Ensure this character hasn't already been drafted
        Debug.Assert(arbChrsAvailableToDraft[(int)chrDrafted] == true);

        arDraftedChrs[iPlayer][arNumChrsDrafted[iPlayer]] = chrDrafted;

        arbChrsAvailableToDraft[(int)chrDrafted] = true;

        draftcollection.SetChrAsDrafted((int)chrDrafted);
        arDraftedChrDisplay[iPlayer].UpdateDraftedChrDisplay(arDraftedChrs[iPlayer]);

    }

    public void Awake() {
        for(int i = 0; i < arDraftedChrs.Length; i++) {
            arDraftedChrs[i] = new Chr.CHARTYPE[NDRAFTEDCHRSPERPLAYER];
        }
    }

    // Start is called before the first frame update
    void Start() {

        InitAllDraftingPortraits();

    }

    // Update is called once per frame
    void Update() {

    }
}
