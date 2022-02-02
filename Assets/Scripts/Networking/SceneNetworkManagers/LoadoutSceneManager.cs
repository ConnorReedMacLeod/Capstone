using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class LoadoutSceneManager : MonoBehaviour {

    void Start() {

        if (PhotonNetwork.IsMasterClient == false) {
            Debug.Log("No need to instantiate another LoadoutNetworkManager if we're not the master client");
            return;
        }
        Debug.Log("Requesting for networkconnectionmanager to add pfLoadoutNetworkManager");
        NetworkConnectionManager.Get().SpawnSceneNetworkManager("pfLoadoutNetworkManager");
    }
}
