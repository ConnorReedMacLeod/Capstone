using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewExecuteButton : MonoBehaviour {

    public static Subject subAllExecuteEvent = new Subject();

	public void OnMouseDown(){
        subAllExecuteEvent.NotifyObs(this);
	}

}
