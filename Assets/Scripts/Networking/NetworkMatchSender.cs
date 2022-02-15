using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkMatchSender : Singleton<NetworkMatchSender> {

    PhotonView photonview;


    public void SendInput(int indexInput, MatchInput matchinputToSend) {

        Debug.LogFormat(LibDebug.AddColor("Sending selection: {0}", LibDebug.Col.BLUE), matchinputToSend);

        photonview.RPC("ReceiveMatchInput", RpcTarget.AllBufferedViaServer, indexInput, matchinputToSend.Serialize());

    }

    //Send the input for the current input that we have processed up to (according to the networkreceiver)
    public void SendNextInput(MatchInput matchinputToSend) {
        SendInput(NetworkMatchReceiver.Get().indexCurMatchInput, matchinputToSend);
    }

    public override void Init() {

        photonview = PhotonView.Get(this);
    }
}
