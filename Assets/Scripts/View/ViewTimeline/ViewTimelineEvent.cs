using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ViewTimelineEvent : Observer {

	public Vector3 v3Pos;

	public float fWidth;
	public float fHeight;


	//undoes the scaling of the parent
	public void Unscale(){
		transform.localScale = new Vector3
			(transform.localScale.x / transform.parent.localScale.x,
				transform.localScale.y / transform.parent.localScale.y,
				transform.localScale.z / transform.parent.localScale.z);
	}
		
	public virtual Vector3 GetPosAfter(){
		return new Vector3(v3Pos.x, v3Pos.y - fHeight - ViewTimeline.fEventGap, v3Pos.z);
	}

	public virtual void SetMaterial(string sMatName){
		string sMatPath = "Materials/Timeline/" + sMatName;
		Material matEvent = Resources.Load(sMatPath, typeof(Material)) as Material;

		GetComponentInChildren<Renderer> ().material = matEvent;
	}

	public abstract int GetPlace ();

	public void SetPos(Vector3 _v3Pos){
		v3Pos = _v3Pos;
		this.transform.localPosition = v3Pos;
	}

	public override void UpdateObs(){
		//Anything that needs to be done across any type of event can be here
		//Will generally just use the specific type's UpdateObs()
	}

	public abstract void Print ();


	//TODO:: It is a problem that view Start methods might be called
	//       after some Notify methods are called.  A possible fix is to intentially
	//       not call Notifys in Start methods - would require some reworking
	public virtual void Start(){

		/*
		Vector3 size = GetComponentInChildren<Renderer> ().bounds.size;
	
		fWidth = size.x;
		fHeight = size.y;*/
	}

}
