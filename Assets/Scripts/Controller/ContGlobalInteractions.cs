using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContGlobalInteractions : Singleton<ContGlobalInteractions> {


    public static Subject subGlobalRightClick = new Subject(Subject.SubType.ALL);

    public override void Init() {
        //Nothing special to do 
    }

    // Update is called once per frame
    void Update() {
        if(Input.GetMouseButtonUp(1)) {
            Debug.Log("Got a right click and have " + subGlobalRightClick.lstCallbacks.Count + " observers");
            subGlobalRightClick.NotifyObs(this);
        }
    }
}
