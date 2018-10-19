﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE:: This is the base class for all Views that support
//        mouse interaction - just override the methods you want
//        to change and ensure these overrides also call the base method

// TODO:: Add right mouse button support
// TODO:: Consider if clicking should immediately send an event, or if it
//        should wait to see if it's a double click before sending the event
public class ViewInteractive : MonoBehaviour {

	public enum STATELEFT {IDLE, PRESS, CLICK, DOUBLEPRESS, DOUBLECLICK, HELD, DRAG};
	public STATELEFT stateLeft;

	public bool bDown; // If the mouse is currently down
	public bool bHeld; // If the mouse has been held down for a while
	public Vector3 v3Down; // Where the mouse was originally pressed down
	public float fTimeDown; // How long the mouse has been pressed down
	public float fTimeUp; // How long the mouse has not been pressed
	public static float fTimeDownDelay; // Delay until pressing down counts as holding the mouse
	public static float fTimeDoubleDelay; // Window to make a double click
	public static float fMinDistDrag; // Distance you have to move the mouse before it counts as dragging

    public Subject subMouseClick;
    public virtual void onMouseClick(params object[] args) {
        subMouseClick.NotifyObs(this, args);
    }

    public Subject subMouseDoubleClick;
    public virtual void onMouseDoubleClick(params object[] args) {
        subMouseDoubleClick.NotifyObs(this, args);
    }

    public Subject subMouseStartHold;
    public virtual void onMouseStartHold(params object[] args) {
        subMouseStartHold.NotifyObs(this, args);
    }

    public Subject subMouseStopHold;
    public virtual void onMouseStopHold(params object[] args) {
        subMouseStopHold.NotifyObs(this, args);
    }

    public Subject subMouseStartDrag;
    public virtual void onMouseStartDrag(params object[] args) {
        subMouseStartDrag.NotifyObs(this, args);
    }

    public Subject subMouseStopDrag;
    public virtual void onMouseStopDrag(params object[] args) {
        subMouseStopDrag.NotifyObs(this, args);
    }

    public Subject subMouseStartHover;
    public virtual void onMouseStartHover(params object[] args) {
        subMouseStartHover.NotifyObs(this, args);
    }

    public Subject subMouseStopHover;
    public virtual void onMouseStopHover(params object[] args) {
        subMouseStopHover.NotifyObs(this, args);
    }

    public Subject subMouseRightClick;
    public static Subject subGlobalMouseRightClick;
    public virtual void onMouseRightClick(params object[] args) {
        subMouseRightClick.NotifyObs(this, args);
        subGlobalMouseRightClick.NotifyObs(this, args);
    }

    public Subject subMouseReleaseOther;
    public virtual void onMouseReleaseOther(params object[] args) {
        subMouseReleaseOther.NotifyObs(this, args);
    }

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

				onMouseStartHold();
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
					onMouseStartHold();
				}
				onMouseStartDrag();
			}

			break;
		}

		if (stateLeft == STATELEFT.DRAG) {

			// Check if we've dragged off of our object then released on another object
			if (Input.GetMouseButtonUp (0)) {

				GameObject goReleasedOver = LibView.GetObjectUnderMouse ();
				if (this.gameObject != goReleasedOver) {
					// Then we've released the mouse over a new object after dragging
                    onMouseReleaseOther(goReleasedOver);

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

			onMouseClick();
			break;

		case STATELEFT.DOUBLEPRESS:
			stateLeft = STATELEFT.DOUBLECLICK;

			onMouseDoubleClick();
			break;

		case STATELEFT.HELD:
			stateLeft = STATELEFT.IDLE;

			onMouseStopHold();
			break;

		case STATELEFT.DRAG:
			stateLeft = STATELEFT.IDLE;

			// For dragging, send a notification that dragging and holding has stopped
			onMouseStopHold();
			onMouseStopDrag();
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
		onMouseStartHover();
	}

	public void OnMouseExit(){
		onMouseStopHover();
	}
}