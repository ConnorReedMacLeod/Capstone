using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPosition : ViewInteractive {

    public Position mod;



    public void UpdateChrOnPositionToHere(Object target, params object[] args) {

        if(mod.chrOnPosition == null) return;

        //Move their global position to our global position
        mod.chrOnPosition.view.transform.position = this.transform.position;

    }


    public override void Start() {

        mod.Start();

        mod.subChrEnteredPosition.Subscribe(UpdateChrOnPositionToHere);

        base.Start();
    }
}
