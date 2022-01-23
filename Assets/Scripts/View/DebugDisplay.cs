using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class DebugDisplay : SingletonPersistent<DebugDisplay> {

    public Text txtDebug;
    public Text txtIsMaster;

    public bool bCachedIsMaster;


    public override void Init() {
        UpdateIsMaster();
    }

    public void SetDebugText(string sDebugText) {
        txtDebug.text = sDebugText;
    }

    void UpdateIsMaster() {
        bCachedIsMaster = PhotonNetwork.IsMasterClient;

        txtIsMaster.text = string.Format("IsMaster({0})", bCachedIsMaster);
    }

    // Update is called once per frame
    void Update() {
        if (bCachedIsMaster != PhotonNetwork.IsMasterClient) {
            UpdateIsMaster();
        }
    }
}
