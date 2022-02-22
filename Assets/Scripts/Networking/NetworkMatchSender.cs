using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkMatchSender : Singleton<NetworkMatchSender> {

    PhotonView photonview;


    public void SendInput(int indexInput, MatchInput matchinputToSend) {
        //Ensure we have started and initialized ourselves before trying to handle any networking duties
        this.Start();

        Debug.LogFormat(LibDebug.AddColor("Sending selection: {0}", LibDebug.Col.BLUE), matchinputToSend);

        photonview.RPC("ReceiveMatchInput", RpcTarget.AllBufferedViaServer, indexInput, matchinputToSend.Serialize());

    }

    public void SendInput(int indexInput, int[] arSerializedMatchInput) {
        //Ensure we have started and initialized ourselves before trying to handle any networking duties
        this.Start();
        
        Debug.LogFormat(LibDebug.AddColor("Sending serialized selection: {0}", LibDebug.Col.BLUE),  LibConversions.ArToStr(arSerializedMatchInput));

        photonview.RPC("ReceiveMatchInput", RpcTarget.AllBufferedViaServer, indexInput, arSerializedMatchInput);
    }

    //Send the input for the current input that we have processed up to (according to the networkreceiver)
    public void SendNextInput(MatchInput matchinputToSend) {
        SendInput(NetworkMatchReceiver.Get().indexCurMatchInput, matchinputToSend);
    }

    public override void Init() {

        photonview = PhotonView.Get(this);
    }
}
