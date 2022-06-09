using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewTimer : Singleton<ViewTimer> {

    public GameObject goTimerBar;
    public Text txtLabel;
    public Text txtTime;

    public float fMaxTime;
    public float fCurTime;

    public Vector3 v3OnScreen;
    public Vector3 v3Offscreen;

    public string sLabel;

    public void SetLabel() {
        txtLabel.text = sLabel;
    }

    public void InitTimer(float _fMaxTime, string _sLabel) {
        sLabel = _sLabel;

        fMaxTime = _fMaxTime;
        fCurTime = fMaxTime;

        SetLabel();
        SetBarWidth();

        //Move this back on screen (in case it was previously hidden)
        this.transform.position = v3OnScreen;
    }


    public void SetBarWidth() {
        goTimerBar.transform.localScale =
            new Vector3((fCurTime / fMaxTime),
                         goTimerBar.transform.localScale.y,
                         goTimerBar.transform.localScale.z);

        txtTime.text = fCurTime.ToString("N");
    }

	
	// Update is called once per frame
	void Update () {
        //Don't need to shorten anything if we're not supposed to be showing anything
        if (fMaxTime == 0f) return;

        fCurTime -= ContTime.Get().fDeltaTime;

        SetBarWidth();

        if (fCurTime < 0) {
            //Move this offscreen to hide it
            this.transform.position = v3Offscreen;

            //Set our maxtime to 0 to signal that we aren't timing anything
            fMaxTime = 0f;
        }
        
	}

    public override void Init() {
        //Save our on-screen position as the starting one from the inspector
        v3OnScreen = this.transform.position;

        //Then move ourself offscreen til we're needed
        this.transform.position = v3Offscreen;

        fMaxTime = 0f;
    }
}
