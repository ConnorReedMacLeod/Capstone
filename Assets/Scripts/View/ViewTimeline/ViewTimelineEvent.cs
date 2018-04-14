using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class ViewTimelineEvent<EventType> : Observer/*, ViewEventInterface*/
	where EventType : TimelineEvent {

	bool bStarted;

	public Vector3 v3Pos;

	public TimelineEvent.STATE stateLast;

	public float fWidth;
	public float fHeight;

	public EventType mod;

	/*public EventType GetMod(){
		return mod;
	}*/


	//Find the model, and do any setup for reflect it
	public void InitModel(){
		mod = GetComponent<EventType>();
		mod.Subscribe (this);

	}

	//undoes the scaling of the parent
	public void Unscale(){
		transform.localScale = new Vector3
			(transform.localScale.x / transform.parent.localScale.x,
				transform.localScale.y / transform.parent.localScale.y,
				transform.localScale.z / transform.parent.localScale.z);
	}


	public abstract float GetVertSpan ();

	public virtual Vector3 GetPosAfter(){
		//Ask the specific type of event what its height + gap are

		return new Vector3(v3Pos.x, v3Pos.y - GetVertSpan(), v3Pos.z);
	}

	public virtual void SetMaterial(string sMatName){
		string sMatPath = "Materials/Timeline/" + sMatName;
		Material matEvent = Resources.Load(sMatPath, typeof(Material)) as Material;

		GetComponentInChildren<Renderer> ().material = matEvent;
	}

	public TimelineEvent.STATE GetState (){
		return mod.state;
	}

	public void SetPos(Vector3 _v3Pos){
		v3Pos = _v3Pos;
		this.transform.localPosition = v3Pos;
	}

	public override void UpdateObs(string eventType, Object target, params object[] args){
		//Anything that needs to be done across any type of event can be here
		//Will generally just use the specific type's UpdateObs()

		switch (eventType) {
		case Notification.EventMoved:
			//Place this event based on the position of the previous node
			if (mod.nodeEvent.Previous == null) {
				//Then we're the first thing in the list
				SetPos(Vector3.zero);
			} else {
				//Place ourselves right after the previous node
				SetPos (mod.nodeEvent.Previous.Value.GetPosAfter ());
			}
			break;

		case Notification.EventChangedState:
			
			Renderer render = GetComponentsInChildren<Renderer>()[0];
			render.material.EnableKeyword ("_EMISSION");

			switch (GetState()){
			case TimelineEvent.STATE.CURRENT:
				//BUG:: The first event doesn't get highlighted - weird right?
				render.material.SetColor ("_EmissionColor", Color.yellow);
				break;
			case TimelineEvent.STATE.FINISHED:
				render.material.DisableKeyword ("_EMISSION");
				break;
			case TimelineEvent.STATE.READY:

				break;
			case TimelineEvent.STATE.UNREADY:

				break;
			default:
				break;
			}
			break;
		default:
			
			break;
		}

	}


	//TODO:: It is a problem that view Start methods might be called
	//       after some Notify methods are called.  A possible fix is to intentially
	//       not call Notifys in Start methods - would require some reworking
	public virtual void Start(){

		if (bStarted == false) {
			bStarted = true;


			/*
			Vector3 size = GetComponentInChildren<Renderer> ().bounds.size;
	
			fWidth = size.x;
			fHeight = size.y;*/
			InitModel ();
		}
	}

}
