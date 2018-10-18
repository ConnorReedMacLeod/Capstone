using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO:: Consider making this a base class from which all views derive
//       can handle all of these built-in actions and only extend the ones you want

// NOTE:: This works essentially like a specific subject, but it only expects one thing to 
//        ever watch it (the owner).  Maybe consider adding static notifications as well?

// TODO:: Add right mouse button support
// TODO:: Consider if clicking should immediately send an event, or if it
//        should wait to see if it's a double click before sending the event
public class MouseHandler : MonoBehaviour {

	public enum STATELEFT {IDLE, PRESS, CLICK, DOUBLEPRESS, DOUBLECLICK, HELD, DRAG};
	public STATELEFT stateLeft;

	public MonoBehaviour owner; //TODO:: Consider if this is even needed
	public System.Type typeOwner;

	public bool bDown; // If the mouse is currently down
	public bool bHeld; // If the mouse has been held down for a while
	public Vector3 v3Down; // Where the mouse was originally pressed down
	public float fTimeDown; // How long the mouse has been pressed down
	public float fTimeUp; // How long the mouse has not been pressed
	public static float fTimeDownDelay; // Delay until pressing down counts as holding the mouse
	public static float fTimeDoubleDelay; // Window to make a double click
	public static float fMinDistDrag; // Distance you have to move the mouse before it counts as dragging

	public Subject.FnCallback fnMouseClick;
	public Subject.FnCallback fnMouseDoubleClick;

	public Subject.FnCallback fnMouseStartHold;
	public Subject.FnCallback fnMouseStopHold;

	public Subject.FnCallback fnMouseStartDrag;
	public Subject.FnCallback fnMouseStopDrag;

	public Subject.FnCallback fnMouseStartHover;
	public Subject.FnCallback fnMouseStopHover;

	public Subject.FnCallback fnMouseRightClick;

	public Subject.FnCallback fnMouseReleaseOther;

	// Use this for initialization
	void Start () {
		stateLeft = STATELEFT.IDLE;
		fTimeDownDelay = 0.2f;
		fTimeDoubleDelay = 0.3f;
		fMinDistDrag = 1.0f;
        Physics.queriesHitTriggers = true;
	}


	void Update () {
		if (bDown) {
			fTimeDown += Time.deltaTime;
		} else {
			fTimeUp += Time.deltaTime;
		}

		// Handle time related checks
		switch (stateLeft) {
		case STATELEFT.PRESS:
		case STATELEFT.DOUBLEPRESS:
			// Check if it's been pressed long enough to count as holding the mouse
			if (fTimeDown >= fTimeDownDelay) {
				bHeld = true;
				stateLeft = STATELEFT.HELD;

				SendNotification (fnMouseStartHold);
			}

			break;

		case STATELEFT.CLICK:
		case STATELEFT.DOUBLECLICK:
			if (fTimeUp >= fTimeDoubleDelay) {
				stateLeft = STATELEFT.IDLE;
			}

			break;
		}
			
		// Handle movement related checks
		switch (stateLeft) {
		case STATELEFT.PRESS:
		case STATELEFT.DOUBLEPRESS:
		case STATELEFT.HELD:
			// Check if the mouse has moved far enough to count as dragging
			if (Vector3.Distance (v3Down, LibView.GetMouseLocation ()) >= fMinDistDrag) {
				stateLeft = STATELEFT.DRAG;

				if (stateLeft != STATELEFT.HELD) {
					SendNotification (fnMouseStartHold);
				}
				SendNotification (fnMouseStartDrag);
			}

			break;
		}

		if (stateLeft == STATELEFT.DRAG) {

			// Check if we've dragged off of our object then released on another object
			if (Input.GetMouseButtonUp (0)) {

				GameObject goReleasedOver = LibView.GetObjectUnderMouse ();
				if (this.gameObject != goReleasedOver) {
					// Then we've released the mouse over a new object after dragging
                    SendNotification(fnMouseReleaseOther, goReleasedOver);

					OnLeftUp ();
				}
				//Otherwise we've released the mouse over ourselves, so we'll catch this with StopDrag
			}
		}
	}

	public void OnLeftDown(){

		bDown = true;
		fTimeDown = 0.0f;
		v3Down = LibView.GetMouseLocation ();

		switch (stateLeft) {
		case STATELEFT.IDLE:
			stateLeft = STATELEFT.PRESS;
			break;

		case STATELEFT.CLICK:
		case STATELEFT.DOUBLECLICK:
			stateLeft = STATELEFT.DOUBLEPRESS;
			break;
		}
	}

	public void OnLeftUp(){

		bDown = false;
		bHeld = false;
		v3Down = Vector3.zero;
		fTimeUp = 0.0f;

		switch (stateLeft) {
		case STATELEFT.PRESS:
			stateLeft = STATELEFT.CLICK;

			SendNotification (fnMouseClick);
			break;

		case STATELEFT.DOUBLEPRESS:
			stateLeft = STATELEFT.DOUBLECLICK;

			SendNotification (fnMouseDoubleClick);
			break;

		case STATELEFT.HELD:
			stateLeft = STATELEFT.IDLE;

			SendNotification (fnMouseStopHold);
			break;

		case STATELEFT.DRAG:
			stateLeft = STATELEFT.IDLE;

			// For dragging, send a notification that dragging and holding has stopped
			SendNotification (fnMouseStopHold);
			SendNotification (fnMouseStopDrag);
			break;
		}

	}

	public void OnMouseOver(){

		if (Input.GetMouseButtonDown (0)) {
			OnLeftDown ();
		} else if (Input.GetMouseButtonUp (0)) {
			OnLeftUp ();
		}
		// TODO:: Add in right-mouse event checks here
	}

	public void OnMouseEnter(){
		SendNotification (fnMouseStartHover);
	}

	public void OnMouseExit(){
		SendNotification (fnMouseStopHover);
	}

	public void SendNotification(Subject.FnCallback fnCallback, params object[] args){
		if (fnCallback != null) {
            //If this notification has been enabled, then send it
			fnCallback(owner, LibView.GetMouseLocation(), args);
		}
	}

	public void SetOwner(MonoBehaviour _owner){
		owner = _owner;
		typeOwner = owner.GetType ();
	}
		
	// Owner scripts should set notifications for the events they want to support
	public void SetNtfClick(Subject.FnCallback _fnMouseClick){
		fnMouseClick = _fnMouseClick;
	}
	public void SetNtfDoubleClick(Subject.FnCallback _fnMouseDoubleClick) {
		fnMouseDoubleClick = _fnMouseDoubleClick;
	}
	public void SetNtfStartHold(Subject.FnCallback _fnMouseStartHold) {
		fnMouseStartHold = _fnMouseStartHold;
	}
	public void SetNtfStopHold(Subject.FnCallback _fnMouseStopHold) {
		fnMouseStopHold = _fnMouseStopHold;
	}
	public void SetNtfStartDrag(Subject.FnCallback _fnMouseStartDrag) {
		fnMouseStartDrag = _fnMouseStartDrag;
	}
	public void SetNtfStopDrag(Subject.FnCallback _fnMouseStopDrag) {
		fnMouseStopDrag = _fnMouseStopDrag;
	}
	public void SetNtfStartHover(Subject.FnCallback _fnMouseStartHover) {
		fnMouseStartHold = _fnMouseStartHover;
	}
	public void SetNtfStopHover(Subject.FnCallback _fnMouseStopHover) {
		fnMouseStopHold = _fnMouseStopHover;
	}
	public void SetReleaseOtherCallback(Subject.FnCallback _fnMouseReleaseOther) {
        fnMouseReleaseOther = _fnMouseReleaseOther;
	}
}
