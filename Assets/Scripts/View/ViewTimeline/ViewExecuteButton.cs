using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewExecuteButton : MonoBehaviour {


	public void OnMouseDown(){
		Controller.Get().NotifyObs(Notification.ClickExecute, null);
	}

}
