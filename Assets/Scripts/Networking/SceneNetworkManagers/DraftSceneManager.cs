using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftSceneManager : MonoBehaviour {

    
    void Start() {
        Debug.Log("Requesting for networkconnectionmanager to add pfDraftNetworkManager");
        NetworkConnectionManager.Get().SpawnSceneNetworkManager("pfDraftNetworkManager");
    }
    
}
