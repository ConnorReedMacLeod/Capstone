using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewManaDate : MonoBehaviour {

    public Text txtManaScheduled;
    public Image imgDateBackground;

    public Sprite sprDateInactive;
    public Sprite sprDateActive;

    public ManaDate modManaDate;

    public void Start() {
        //Ensure the model is initialized before us
        modManaDate.Start();

        modManaDate.pmanaScheduled.subChanged.Subscribe(cbOnScheduleManaChange);
        modManaDate.subBecomeActiveDate.Subscribe(cbOnBecomeActiveDate);
        modManaDate.subBecomeInactiveDate.Subscribe(cbOnBecomeInactiveDate);

        cbOnScheduleManaChange(null);
    }

    public void cbOnScheduleManaChange(Object tar, params object[] args) {
        SetScheduledManaText(modManaDate.pmanaScheduled.Get());
    }

    public void SetScheduledManaText(Mana manaScheduled) {
        Mana manaTotalScheduled = new Mana(manaScheduled);

        txtManaScheduled.text = manaTotalScheduled.ToPrettyString();
    }

    public void cbOnBecomeActiveDate(Object tar, params object[] args) {
        imgDateBackground.sprite = sprDateActive;
    }

    public void cbOnBecomeInactiveDate(Object tar, params object[] args) {
        imgDateBackground.sprite = sprDateInactive;
    }
}
