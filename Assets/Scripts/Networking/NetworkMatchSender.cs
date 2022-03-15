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

    public void SendInput(int indexInput, int[] arSerializedMatchInput) {
        
        Debug.LogFormat(LibDebug.AddColor("Sending serialized selection: {0}", LibDebug.Col.BLUE),  LibConversions.ArToStr(arSerializedMatchInput));

        photonview.RPC("ReceiveMatchInput", RpcTarget.AllBufferedViaServer, indexInput, arSerializedMatchInput);
    }

    //Send the input for the current input that we have processed up to (according to the networkreceiver)
    public void SendNextInput(MatchInput matchinputToSend) {
        SendInput(NetworkMatchReceiver.Get().indexCurMatchInput, matchinputToSend);
    }

    protected override void Awake() {
        base.Awake();
        Debug.Log("Awaking NetworkMatchSender");
    }

    public override void Start() {
        base.Start();
        Debug.Log("Starting NetworkMatchSender");
    }

    public override void Init() {
        Debug.Log("Initing NetworkMatchSender");
        photonview = PhotonView.Get(this);
    }
}
