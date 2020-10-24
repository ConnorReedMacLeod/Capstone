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

    public void DisplayAction (Action act) {
        if (bLocked == false) {
            viewInfoPanel.ShowInfoAction(act);
        }
    }

    public void cbSoulStartHover(Object target, params object[] args) {
        if (((ViewSoul)target).mod == null || ((ViewSoul)target).mod.actSource == null) {
            //Debug.Log("No action source to display");
        } else {
            //Debug.Log("Displaying " + ((ViewSoul)target).mod.actSource.sName);
            DisplayAction(((ViewSoul)target).mod.actSource);
        }
    }

    public void cbActStartHover(Object target, params object[] args) {
        DisplayAction(((ViewAction)target).mod);
    }

    public void cbBlockerButtonStartHover(Object target, params object[] args) {
        DisplayAction(ContTurns.Get().GetNextActingChr().arActions[Chr.idBlocking]);
    }

    public void cbRestButtonStartHover(Object target, params object[] args) {
        DisplayAction(ContTurns.Get().GetNextActingChr().arActions[Chr.idResting]);
    }

    public void StopDisplayAction(Action act) {
        if (bLocked == false && 
            ((viewInfoPanel.viewInfoAction == null) || //If nothing is currently being shown
            act == viewInfoPanel.viewInfoAction.mod)) {
            // First ensure that what we're leaving is the current displayed ability
            //When we stop hovering over the thing we're displaying, stop displaying it
            viewInfoPanel.ClearPanel();
        }
    }

    public void cbSoulStopHover(Object target, params object[] args) {
        if (((ViewSoul)target).mod == null) {
            StopDisplayAction(null);
        } else {
            StopDisplayAction(((ViewSoul)target).mod.actSource);
        }
    }

    public void cbActStopHover(Object target, params object[] args) {
        StopDisplayAction(((ViewAction)target).mod);
    }

    public void cbBlockerButtonStopHover(Object target, params object[] args) {
        StopDisplayAction(ContTurns.Get().GetNextActingChr().arActions[Chr.idBlocking]);
    }

    public void cbRestButtonStopHover(Object target, params object[] args) {
        StopDisplayAction(ContTurns.Get().GetNextActingChr().arActions[Chr.idResting]);
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

            ContLocalUIInteraction.subAllStartTargetting.Subscribe(cbStartTargetting);
            ContLocalUIInteraction.subAllFinishTargetting.Subscribe(cbFinishTargetting);

            ViewAction.subAllStartHover.Subscribe(cbActStartHover);
            ViewAction.subAllStopHover.Subscribe(cbActStopHover);

            ViewSoul.subAllStartHover.Subscribe(cbSoulStartHover);
            ViewSoul.subAllStopHover.Subscribe(cbSoulStopHover);

            ViewBlockerButton.subAllStartHover.Subscribe(cbBlockerButtonStartHover);
            ViewBlockerButton.subAllStopHover.Subscribe(cbBlockerButtonStopHover);

            ViewRestButton.subAllStartHover.Subscribe(cbRestButtonStartHover);
            ViewRestButton.subAllStopHover.Subscribe(cbRestButtonStopHover);
        }
	}
}
