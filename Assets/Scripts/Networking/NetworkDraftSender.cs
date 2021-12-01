using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkDraftSender : Singleton<NetworkDraftSender> {

    PhotonView photonview;


    public void SendBan(CharType.CHARTYPE chartypeToBan) {

        int indexCurDraftInput = NetworkDraftReceiver.Get().indexCurDraftInput;

        Debug.LogFormat("Sending step {0}: Ban {1}", indexCurDraftInput, chartypeToBan);

        photonview.RPC("ReceiveBan", RpcTarget.AllBufferedViaServer, indexCurDraftInput, chartypeToBan);

    }

    public void SendDraft(CharType.CHARTYPE chartypeToDraft) {

        int indexCurDraftInput = NetworkDraftReceiver.Get().indexCurDraftInput;

        Debug.LogFormat("Sending step {0}: Draft {1}", indexCurDraftInput, chartypeToDraft);

        photonview.RPC("ReceiveDraft", RpcTarget.AllBufferedViaServer, indexCurDraftInput, chartypeToDraft);
    }

    public override void Init() {

        photonview = PhotonView.Get(this);
    }
}
