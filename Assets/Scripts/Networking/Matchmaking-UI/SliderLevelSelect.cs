using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class SliderLevelSelect : MonoBehaviour {

    public Text txtLevelLabel;

    // Start is called before the first frame update
    void Start() {
        OnLevelChange(0);
    }

    public void SetLevelLabel(int nLevel) {
        txtLevelLabel.text = "Level: " + nLevel.ToString();
    }

    public void OnLevelChange(float fLevel) {
        NetworkConnectionManager.nMyLevel = Mathf.FloorToInt(fLevel);
        SetLevelLabel(NetworkConnectionManager.nMyLevel);
    }
}
