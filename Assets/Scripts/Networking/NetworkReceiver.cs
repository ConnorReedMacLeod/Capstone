using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkReceiver : Singleton<NetworkReceiver> {

    public const int NDEFAULTSELECTIONSCAPACITY = 100;

    public int indexCurSelection;
    public List<Selections> lstSelectionsBuffer;


    [PunRPC]
    void ReceiveSkillSelection(int indexInput, int[] arnSerializedSelection) {

        if(indexInput != indexCurSelection) {
            Debug.LogErrorFormat("ALERT!  Received input index {0}, but we are expecting index {1}", indexInput, indexCurSelection);
        }

        //Ensure that our received index is within the bounds of our buffer
        while(indexInput > lstSelectionsBuffer.Count) {
            IncreaseSelectionsReceivedCapacity();
        }

        //Deserialize the passed selections
        Selections selectionsReceived = new Selections(arnSerializedSelection);

        //Check if this entry in the buffer is already filled
        if (lstSelectionsBuffer[indexInput] != null) {
            Debug.LogErrorFormat("ALERT! Filled index {0} received another selection of {1}", indexInput, selectionsReceived);
            return;
        }

        lstSelectionsBuffer[indexInput] = selectionsReceived;

        //If we've received the selection we've been waitign for, then react to that selection
        if(indexInput == indexCurSelection) {
            ReceivedPendingSelection();
        }

    }

    public void ReceivedPendingSelection() {
        //todo - generally process the events of the skill selection
    }

    public bool IsCurSelectionReady() {
        return lstSelectionsBuffer[indexCurSelection] != null;
    }

    public Selections GetCurSelection() {
        Debug.Assert(IsCurSelectionReady());

        return lstSelectionsBuffer[indexCurSelection];
    }

    //To be called once execution of the current skill is completely finished
    public void FinishCurSelection() {
        indexCurSelection++;
    }

    // Increase the number of selections that can be stored by the default amount
    public void IncreaseSelectionsReceivedCapacity() {
        for (int i = 0; i < NDEFAULTSELECTIONSCAPACITY; i++) {
            lstSelectionsBuffer.Add(null);
        }
    }

    public override void Init() {
        lstSelectionsBuffer = new List<Selections>(NDEFAULTSELECTIONSCAPACITY);
        IncreaseSelectionsReceivedCapacity();
    }

}
