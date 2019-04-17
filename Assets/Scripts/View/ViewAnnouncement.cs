using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewAnnouncement : Singleton<ViewAnnouncement> {

    public Text txtTime;

    public string sAnnouncement;

    public float fMaxTime;
    public float fCurTime;

    public string sLabel;

    private void Start() {
        sAnnouncement = "";
        SetText();
    }


    public void SetText() {
        txtTime.text = sAnnouncement;
    }

    public void InitAnnouncement(float _fMaxTime, string _sAnnouncement) {
        sAnnouncement = _sAnnouncement;

        fMaxTime = _fMaxTime;
        fCurTime = 0.0f;

        SetText();
    }


    // Update is called once per frame
    void Update() {
        fCurTime += ContTime.Get().fDeltaTime;

        if (fCurTime >= fMaxTime) {
            sAnnouncement = "";
            SetText();
        }
        
    }
}