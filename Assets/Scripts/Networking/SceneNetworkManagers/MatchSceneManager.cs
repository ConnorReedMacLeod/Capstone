using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSceneManager : MonoBehaviour {

    void Start() {
        Debug.Log("Requesting for networkconnectionmanager to add pfMatchNetworkManager");
        NetworkConnectionManager.Get().SpawnSceneNetworkManager("pfMatchNetworkManager");
    }
}
