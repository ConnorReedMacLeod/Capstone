using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkSender : Singleton<NetworkSender> {

    PhotonView photonview;


    public void SendSkillSelection(int indexInput, Selections selectionsToSend) {

        Debug.LogFormat("Sending selection: {0}", selectionsToSend);

        photonview.RPC("ReceiveSkillSelection", RpcTarget.AllBufferedViaServer, selectionsToSend.GetSerialization());

    }

    public override void Init() {

        photonview = PhotonView.Get(this);
    }
}
