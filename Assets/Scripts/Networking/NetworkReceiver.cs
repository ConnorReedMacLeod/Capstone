﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkReceiver : Singleton<NetworkReceiver> {

    public const int NDEFAULTSELECTIONSCAPACITY = 100;

    public int indexCurMatchInput;
    public List<MatchInput> lstMatchInputBuffer;


    [PunRPC]
    void ReceiveSkillSelection(int indexInput, int[] arnSerializedSelection) {

        //Deserialize the passed selections
        InputSkillSelection selectionsReceived = new InputSkillSelection(arnSerializedSelection);

        AddInputToBuffer(indexInput, selectionsReceived);
    }


    void AddInputToBuffer(int indexInput, MatchInput matchInput) {

        if(indexInput != indexCurMatchInput) {
            Debug.LogErrorFormat("ALERT!  Received input index {0}, but we are expecting index {1}", indexInput, indexCurMatchInput);
        }

        //Ensure that our received index is within the bounds of our buffer
        while(indexInput > lstMatchInputBuffer.Count) {
            IncreaseMatchInputsReceivedCapacity();
        }

        //Check if this entry in the buffer is already filled
        if (lstMatchInputBuffer[indexInput] != null) {
            Debug.LogErrorFormat("ALERT! Filled index {0} received another input of {1}", indexInput, matchInput);
            return;
        }

        lstMatchInputBuffer[indexInput] = matchInput;


    }

    public bool IsCurMatchInputReady() {
        return lstMatchInputBuffer[indexCurMatchInput] != null;
    }

    public MatchInput GetCurMatchInput() {
        Debug.Assert(IsCurMatchInputReady());

        return lstMatchInputBuffer[indexCurMatchInput];
    }

    //To be called once execution of the current skill is completely finished
    public void FinishCurMatchInput() {
        indexCurMatchInput++;
    }

    // Increase the number of selections that can be stored by the default amount
    public void IncreaseMatchInputsReceivedCapacity() {
        for (int i = 0; i < NDEFAULTSELECTIONSCAPACITY; i++) {
            lstMatchInputBuffer.Add(null);
        }
    }

    public override void Init() {
        lstMatchInputBuffer = new List<MatchInput>(NDEFAULTSELECTIONSCAPACITY);
        IncreaseMatchInputsReceivedCapacity();
    }

}
