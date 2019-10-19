using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class DropDownMatchTypeSelect : MonoBehaviour {


    void Start() {
        OnMatchTypeChange(0);
    }

    public void OnMatchTypeChange(int nMatchTypeSelection) {

        NetworkConnectionManager.Get().matchType = (NetworkConnectionManager.MATCHTYPE)nMatchTypeSelection;

        Debug.Log(NetworkConnectionManager.Get().matchType);
    }

}
