using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class DraftSceneManager : MonoBehaviour {

    
    void Start() {

        if (PhotonNetwork.IsMasterClient == false) {
            Debug.Log("No need to instantiate another DraftNetworkManager if we're not the master client");
            return;
        }

        Debug.Log("Requesting for networkconnectionmanager to add pfDraftNetworkManager");
        NetworkConnectionManager.Get().SpawnSceneNetworkManager("pfDraftNetworkManager");
    }
    
}
