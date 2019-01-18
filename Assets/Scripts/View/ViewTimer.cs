using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewTimer : MonoBehaviour {

    //TODO Despawn this is you've manually spawned another Timer;

    public GameObject goTimerBar;
    public Text txtLabel;
    public Text txtTime;

    public float fBarMaxWidth;
    public float fMaxTime;
    public float fCurTime;

    public string sLabel;

    public void SetLabel() {
        txtLabel.text = sLabel;
    }

    public void InitTimer(float _fMaxTime, string _sLabel) {
        sLabel = _sLabel;
        Debug.Log("Making a timer for " + sLabel);
        fMaxTime = _fMaxTime;
        fCurTime = fMaxTime;

        fBarMaxWidth = goTimerBar.transform.localScale.x;

        SetLabel();
        SetBarWidth();
    }


    public void SetBarWidth() {
        goTimerBar.transform.localScale =
            new Vector3(fBarMaxWidth * (fCurTime / fMaxTime),
                         goTimerBar.transform.localScale.y,
                         goTimerBar.transform.localScale.z);

        txtTime.text = fCurTime.ToString("N");
    }

	
	// Update is called once per frame
	void Update () {
        fCurTime -= Time.deltaTime;

        if(fCurTime < 0) {
            Destroy(this.gameObject);
        }

        SetBarWidth();
	}
}
