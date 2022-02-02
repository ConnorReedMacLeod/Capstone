using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class MatchSceneManager : MonoBehaviour {

    void Start() {

        if(PhotonNetwork.IsMasterClient == false) {
            Debug.Log("No need to instantiate another MatchNetworkManager if we're not the master client");
            return;
        }
        Debug.Log("Requesting for networkconnectionmanager to add pfMatchNetworkManager");
        NetworkConnectionManager.Get().SpawnSceneNetworkManager("pfMatchNetworkManager");
    }
}
