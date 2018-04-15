using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContInfo : Observer {

	public enum StateInfo {Action};

	public StateInfo stateInfo;
	public bool bLocked; //TODO:: Flush out a target locking system more

	public ViewInfoPanel viewInfoPanel;

	public override void UpdateObs(string eventType, Object target, params object[] args){
		switch(stateInfo){
		case StateInfo.Action:
			switch (eventType) {
			case Notification.ActStartHover:
				if (bLocked == false) {
					viewInfoPanel.ShowInfoAction (((ViewAction)target).mod.arActions [(int)args [0]]);
				}

				break;

			case Notification.ActStopHover:
				if (bLocked == false &&
				    ((ViewAction)target).mod.arActions [(int)args [0]] == viewInfoPanel.viewInfoAction.mod) {
					//When we stop hovering over the thing we're displaying, stop displaying it
					viewInfoPanel.ClearPanel ();
				}

				break;

			default:

				break;
			}
			break;

		default:

			break;
		}
	}

	public void Start(){
		GameObject go = GameObject.FindGameObjectWithTag ("Info");
		if (go == null) {
			Debug.LogError ("ERROR! NO INFO-TAGGED OBJECT!");
		}
		viewInfoPanel = go.GetComponent<ViewInfoPanel> ();
		if (viewInfoPanel == null) {
			Debug.LogError ("ERROR! NO VIEWINFOPANEL ON INFO-TAGGED OBJECT!");
		}
	}
}
