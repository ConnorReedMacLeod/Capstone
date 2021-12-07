using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkDraftReceiver : Singleton<NetworkDraftReceiver> {

    public const int NDEFAULTDRAFTINPUTS = 25;

    public int indexCurDraftInput;
    public List<CharType.CHARTYPE> lstDraftInputBuffer;


    [PunRPC]
    void ReceiveBan(int indexDraftInput, CharType.CHARTYPE chartypeToBan) {

        Debug.LogFormat("Received input #{0}: Ban {1}", indexCurDraftInput, chartypeToBan);
        AddInputToBuffer(indexDraftInput, chartypeToBan);

    }

    [PunRPC]
    void ReceiveDraft(int indexDraftInput, CharType.CHARTYPE chartypeToDraft) {

        Debug.LogFormat("Received input #{0}: Draft {1}", indexCurDraftInput, chartypeToDraft);
        AddInputToBuffer(indexDraftInput, chartypeToDraft);

    }

    void AddInputToBuffer(int indexDraftInput, CharType.CHARTYPE chartype) {

        if (indexDraftInput != indexCurDraftInput) {
            Debug.LogErrorFormat("ALERT!  Received draftinput index {0}, but we are expecting index {1}", indexDraftInput, indexCurDraftInput);
        }

        //Ensure that our received index is within the bounds of our buffer
        while (indexDraftInput > lstDraftInputBuffer.Count) {
            IncreaseDraftInputsReceivedCapacity();
        }

        //Check if this entry in the buffer is already filled (LENGTH indicates an unfilled selection)
        if (lstDraftInputBuffer[indexDraftInput] != CharType.CHARTYPE.LENGTH) {
            Debug.LogErrorFormat("ALERT! Filled index {0} received another selection of {1}", indexDraftInput, chartype);
            return;
        }

        lstDraftInputBuffer[indexDraftInput] = chartype;
    }

    public bool IsCurSelectionReady() {
        return lstDraftInputBuffer[indexCurDraftInput] != CharType.CHARTYPE.LENGTH;
    }

    public CharType.CHARTYPE GetCurSelection() {
        Debug.Assert(IsCurSelectionReady());

        return lstDraftInputBuffer[indexCurDraftInput];
    }

    //To be called once execution of the current input is completely finished
    public void FinishCurSelection() {
        indexCurDraftInput++;
    }

    // Increase the number of selections that can be stored by the default amount
    public void IncreaseDraftInputsReceivedCapacity() {
        for (int i = 0; i < NDEFAULTDRAFTINPUTS; i++) {
            lstDraftInputBuffer.Add(CharType.CHARTYPE.LENGTH);
        }
    }

    public override void Init() {

        lstDraftInputBuffer = new List<CharType.CHARTYPE>(NDEFAULTDRAFTINPUTS);
        IncreaseDraftInputsReceivedCapacity();
    }
}
