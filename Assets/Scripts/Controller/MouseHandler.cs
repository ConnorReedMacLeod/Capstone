using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO:: Add right mouse button support
// TODO:: Consider if clicking should immediately send an event, or if it
//        should wait to see if it's a double click before sending the event
public class MouseHandler : MonoBehaviour {

	public enum STATELEFT {IDLE, PRESS, CLICK, DOUBLEPRESS, DOUBLECLICK, HELD, DRAG};
	public STATELEFT stateLeft;

	public MonoBehaviour owner; //TODO:: Consider if this is even needed
	public System.Type typeOwner;

	public delegate void fnCallback(); // allow owner to set a function to let it know when an event has happend
	public delegate void fnCallbackGo(GameObject go); // callback for passing a single Gameobject

	public bool bDown; // If the mouse is currently down
	public bool bHeld; // If the mouse has been held down for a while
	public Vector3 v3Down; // Where the mouse was originally pressed down
	public float fTimeDown; // How long the mouse has been pressed down
	public float fTimeUp; // How long the mouse has not been pressed
	public static float fTimeDownDelay; // Delay until pressing down counts as holding the mouse
	public static float fTimeDoubleDelay; // Window to make a double click
	public static float fMinDistDrag; // Distance you have to move the mouse before it counts as dragging

	public string ntfMouseClick;
	public fnCallback fnMouseClick;
	public string ntfMouseDoubleClick;
	public fnCallback fnMouseDoubleClick;

	public string ntfMouseStartHold;
	public fnCallback fnMouseStartHold;
	public string ntfMouseStopHold;
	public fnCallback fnMouseStopHold;

	public string ntfMouseStartDrag;
	public fnCallback fnMouseStartDrag;
	public string ntfMouseStopDrag;
	public fnCallback fnMouseStopDrag;

	public string ntfMouseStartHover;
	public fnCallback fnMouseStartHover;
	public string ntfMouseStopHover;
	public fnCallback fnMouseStopHover;

	public string ntfMouseRightClick;
	public fnCallback fnMouseRightClick;

	//public string ntfMouseRleaseOther;  //TODO:: Consider if a notification for this is even needed
	public fnCallbackGo fnMouseReleaseOther;

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
				if (fnMouseStartHold != null) {
					fnMouseStartHold ();
				}
				SendNotification (ntfMouseStartHold);
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
				if (fnMouseStartDrag != null) {
					fnMouseStartDrag ();
				}
				if (stateLeft != STATELEFT.HELD) {
					SendNotification (ntfMouseStartHold);
				}
				SendNotification (ntfMouseStartDrag);
			}

			break;
		}

		if (stateLeft == STATELEFT.DRAG) {

			// Check if we've dragged off of our object then released on another object
			if (Input.GetMouseButtonUp (0)) {

				GameObject goReleasedOver = LibView.GetObjectUnderMouse ();
				if (this.gameObject != goReleasedOver) {
					// Then we've released the mouse over a new object after dragging
					if (fnMouseReleaseOther != null) {
						fnMouseReleaseOther (goReleasedOver);
					}
					OnLeftUp ();
				}
				//Otherwise we've released the mouse over ourselves, so we'll catch this with StopDrag
			}
		}
	}

	public void OnLeftDown(){
        Debug.Log(owner);

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
			if (fnMouseClick != null) {
				fnMouseClick ();
			}
			SendNotification (ntfMouseClick);
			break;

		case STATELEFT.DOUBLEPRESS:
			stateLeft = STATELEFT.DOUBLECLICK;
			if (fnMouseDoubleClick != null) {
				fnMouseDoubleClick ();
			}
			SendNotification (ntfMouseDoubleClick);
			break;

		case STATELEFT.HELD:
			stateLeft = STATELEFT.IDLE;
			if (fnMouseStopHold != null) {
				fnMouseStopHold ();
			}
			SendNotification (ntfMouseStopHold);
			break;

		case STATELEFT.DRAG:
			stateLeft = STATELEFT.IDLE;

			// For dragging, send a notification that dragging and holding has stopped
			if (fnMouseStopHold != null) {
				fnMouseStopHold ();
			}
			if (fnMouseStopDrag != null) {
				fnMouseStopDrag ();
			}
			SendNotification (ntfMouseStopHold);
			SendNotification (ntfMouseStopDrag);
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
		if (fnMouseStartHover != null) {
			fnMouseStartHover ();
		}

		SendNotification (ntfMouseStartHover);
	}

	public void OnMouseExit(){
		if (fnMouseStopHover != null) {
			fnMouseStopHover ();
		}
		SendNotification (ntfMouseStopHover);
	}

	public void SendNotification(string ntf){
		if (ntf != "") {
            //If this notification has been enabled, then send it
			Controller.Get ().NotifyObs (ntf, owner, LibView.GetMouseLocation());
		}
	}

	public void SetOwner(MonoBehaviour _owner){
		owner = _owner;
		typeOwner = owner.GetType ();
	}
		
	// Owner scripts should set notifications for the events they want to support
	public void SetNtfClick(string _ntfMouseClick, fnCallback _fnMouseClick = null){
		ntfMouseClick = _ntfMouseClick;
		fnMouseClick = _fnMouseClick;
	}
	public void SetNtfDoubleClick(string _ntfMouseDoubleClick, fnCallback _fnMouseDoubleClick = null){
		ntfMouseDoubleClick = _ntfMouseDoubleClick;
		fnMouseDoubleClick = _fnMouseDoubleClick;
	}
	public void SetNtfStartHold(string _ntfMouseStartHold, fnCallback _fnMouseStartHold = null){
		ntfMouseStartHold = _ntfMouseStartHold;
		fnMouseStartHold = _fnMouseStartHold;
	}
	public void SetNtfStopHold(string _ntfMouseStopHold, fnCallback _fnMouseStopHold = null){
		ntfMouseStopHold = _ntfMouseStopHold;
		fnMouseStopHold = _fnMouseStopHold;
	}
	public void SetNtfStartDrag(string _ntfMouseStartDrag, fnCallback _fnMouseStartDrag = null){
		ntfMouseStartDrag = _ntfMouseStartDrag;
		fnMouseStartDrag = _fnMouseStartDrag;
	}
	public void SetNtfStopDrag(string _ntfMouseStopDrag, fnCallback _fnMouseStopDrag = null){
		ntfMouseStopDrag = _ntfMouseStopDrag;
		fnMouseStopDrag = _fnMouseStopDrag;
	}
	public void SetNtfStartHover(string _ntfMouseStartHover, fnCallback _fnMouseStartHover = null){
		ntfMouseStartHover = _ntfMouseStartHover;
		fnMouseStartHold = _fnMouseStartHover;
	}
	public void SetNtfStopHover(string _ntfMouseStopHover, fnCallback _fnMouseStopHover = null){
		ntfMouseStopHover = _ntfMouseStopHover;
		fnMouseStopHold = _fnMouseStopHover;
	}

	public void SetReleaseOtherCallback(fnCallbackGo _fnMouseReleaseOther){
		fnMouseReleaseOther = _fnMouseReleaseOther;
	}
}
