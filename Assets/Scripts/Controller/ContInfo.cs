using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContInfo : MonoBehaviour{

    bool bStarted;

	public enum StateInfo {Action};

	public StateInfo stateInfo; //TODO:: Alternate which subscriptions you are using depending on
                                //       which state you're currently in
	public bool bLocked; //TODO:: Flush out a target locking system more

	public ViewInfoPanel viewInfoPanel;
	public Action actFocus;

    public void cbStartTargetting(Object target, params object[] args) {
        SetActionFocus(((Chr)target).arActions[(int)args[0]]);
        bLocked = true;
    }

    public void cbFinishTargetting(Object target, params object[] args) {
        ClearActionFocus();
        bLocked = false;
    }

    public void cbActStartHover(Object target, params object[] args) {
        if (bLocked == false) {
            viewInfoPanel.ShowInfoAction(((ViewAction)target).mod);
        }
    }

    public void cbActStopHover(Object target, params object[] args) {
        if (bLocked == false && ((ViewAction)target).mod == viewInfoPanel.viewInfoAction.mod) {
            // First ensure that what we're leaving is the current displayed ability
            //When we stop hovering over the thing we're displaying, stop displaying it
            viewInfoPanel.ClearPanel();
        }
    }

	public void SetActionFocus(Action _actFocus){
		viewInfoPanel.ClearPanel ();
		actFocus = _actFocus;
		viewInfoPanel.ShowInfoAction (actFocus);
	}

	public void ClearActionFocus(){
		actFocus = null;
		viewInfoPanel.ClearPanel ();//TODO:: This feels like it could maybe bug
	}

    public void Start() {
        if (!bStarted) {
            bStarted = true;
            GameObject go = GameObject.FindGameObjectWithTag("Info");
            if (go == null) {
                Debug.LogError("ERROR! NO INFO-TAGGED OBJECT!");
            }
            viewInfoPanel = go.GetComponent<ViewInfoPanel>();
            if (viewInfoPanel == null) {
                Debug.LogError("ERROR! NO VIEWINFOPANEL ON INFO-TAGGED OBJECT!");
            }

        ContTarget.subAllStartTargetting.Subscribe(cbStartTargetting);
        ContTarget.subAllFinishTargetting.Subscribe(cbFinishTargetting);

        ViewChr.subAllStartHover.Subscribe(cbActStartHover);
        ViewChr.subAllStopHover.Subscribe(cbActStopHover);
        }
	}
}
