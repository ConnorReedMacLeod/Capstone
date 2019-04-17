using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewAutoTurnsButton : MonoBehaviour {

    public static Subject subAllAutoExecuteEvent = new Subject(Subject.SubType.ALL);

    public void OnMouseDown() {
        subAllAutoExecuteEvent.NotifyObs(this);

        this.gameObject.transform.position = new Vector3(-100f, -100f, 0f);
    }

}
