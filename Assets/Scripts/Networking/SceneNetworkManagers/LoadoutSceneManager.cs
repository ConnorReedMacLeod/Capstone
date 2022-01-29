using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutSceneManager : MonoBehaviour {

    void Start() {
        Debug.Log("Requesting for networkconnectionmanager to add pfLoadoutNetworkManager");
        NetworkConnectionManager.Get().SpawnSceneNetworkManager("pfLoadoutNetworkManager");
    }
}
