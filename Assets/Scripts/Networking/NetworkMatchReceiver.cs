using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkMatchReceiver : Singleton<NetworkMatchReceiver> {

    public const int NDEFAULTSELECTIONSCAPACITY = 100;

    public int indexCurMatchInput;
    public List<MatchInput> lstMatchInputBuffer;


    [PunRPC]
    void ReceiveMatchInput(int indexInput, int[] arnSerializedMatchInput, MatchInput.MatchInputType matchinputtype) {

        //TODO - figure out which deserialization process should be used - always using InputSkillSelection here, but 
        //   we could pass some extra input-type enum along with the input to let us know which matchinput type we should decode into

        //Deserialize the passed selections
        MatchInput selectionsReceived = CreateMatchInput(arnSerializedMatchInput, matchinputtype);

        AddInputToBuffer(indexInput, selectionsReceived);
    }


    //Take the enum for the matchinput type and uses that to call the appropriate constructor for that type
    // - it'd be nice if there's a cleaner way to do this, but I'm not sure how
    MatchInput CreateMatchInput(int[] arnSerializedMatchInput, MatchInput.MatchInputType matchinputtype) {

        switch(matchinputtype) {
        case MatchInput.MatchInputType.SkillSelection:

            return new InputSkillSelection(arnSerializedMatchInput);

        case MatchInput.MatchInputType.ReplaceOpenPos:

            return new InputReplaceEmptyPos(arnSerializedMatchInput);

        default:

            Debug.LogErrorFormat("Error! {0} is an unspported matchinputtype", matchinputtype);
            break;
        }

        return null;
    }


    void AddInputToBuffer(int indexInput, MatchInput matchInput) {

        if(indexInput != indexCurMatchInput) {
            Debug.LogErrorFormat("ALERT!  Received input index {0}, but we are waiting to process index {1}", indexInput, indexCurMatchInput);
        }

        //Ensure that our received index is within the bounds of our buffer
        while(indexInput > lstMatchInputBuffer.Count) {
            IncreaseMatchInputsReceivedCapacity();
        }

        //Check if this entry in the buffer is already filled
        if(lstMatchInputBuffer[indexInput] != null) {
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

    public bool HasNReadyInputs(int n) {
        //Determines if we have at least n inputs waiting in our buffer that we haven't yet processed (i.e., after the current input)

        for(int i = indexCurMatchInput; i < indexCurMatchInput + n; i++) {
            if(i > lstMatchInputBuffer.Count) {
                IncreaseMatchInputsReceivedCapacity();
            }
            if(lstMatchInputBuffer[i] == null) {
                return false;
            }
        }

        return true;
    }

    //To be called once execution of the current skill is completely finished
    public void FinishCurMatchInput() {
        indexCurMatchInput++;
    }

    // Increase the number of selections that can be stored by the default amount
    public void IncreaseMatchInputsReceivedCapacity() {
        for(int i = 0; i < NDEFAULTSELECTIONSCAPACITY; i++) {
            lstMatchInputBuffer.Add(null);
        }
    }

    public override void Init() {
        lstMatchInputBuffer = new List<MatchInput>(NDEFAULTSELECTIONSCAPACITY);
        IncreaseMatchInputsReceivedCapacity();
    }

}
