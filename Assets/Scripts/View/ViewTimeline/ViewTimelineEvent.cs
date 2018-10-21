using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class ViewTimelineEvent : MonoBehaviour {

	bool bStarted;

	public Vector3 v3Pos;

	public TimelineEvent.STATE stateLast;

	public float fWidth;
	public float fHeight;

	public TimelineEvent mod {
        get {
            return GetMod();
        }
        set {
            mod = value;
        }
    }

    public virtual TimelineEvent GetMod() {
        //TODO:: Consider if there's a way to do this without
        //       a unity library function call each time
        return GetComponent<TimelineEvent>();
    }

	public abstract float GetVertSpan ();

	public virtual Vector3 GetPosAfter(){
		//Ask the specific type of event what its height + gap are

		return new Vector3(v3Pos.x, v3Pos.y - GetVertSpan(), v3Pos.z);
	}

	public TimelineEvent.STATE GetState (){
		return mod.state;
	}

	public void SetPos(Vector3 _v3Pos){
		v3Pos = _v3Pos;
		transform.localPosition = v3Pos;
	}

    //This event has been moved (since something on the timeline was inserted
    // before it), so we need to shift this down to be after it's predecessor
    public void cbEventMoved(Object target, params object[] args) {

        if (mod.nodeEvent.Previous == null) {
            //Then we're the first thing in the list
            SetPos(Vector3.zero);
        } else {
            //Place ourselves right after the previous node
            SetPos(mod.nodeEvent.Previous.Value.GetView().GetPosAfter());
        }

        if(mod.nodeEvent.Next != null) {
            //Then let the node below us know that it should shift up as well
            mod.nodeEvent.Next.Value.subEventMoved.NotifyObs();
        }
    }

    public void cbChangedState(Object target, params object[] args) {
        Renderer render = GetComponentsInChildren<Renderer>()[0];
        render.material.EnableKeyword("_EMISSION");

        switch (GetState()) {
            case TimelineEvent.STATE.CURRENT:
                //BUG:: The first event doesn't get highlighted - weird right?
                render.material.SetColor("_EmissionColor", Color.yellow);
                break;
            case TimelineEvent.STATE.FINISHED:
                render.material.DisableKeyword("_EMISSION");
                break;
            case TimelineEvent.STATE.READY:

                break;
            case TimelineEvent.STATE.UNREADY:

                break;
            default:
                break;
        }
    }

	public virtual void Start(){

		if (bStarted == false) {
			bStarted = true;

            //Call these events manually just in case the iinitial notificiation is sent out
            //before we've gotten a change to subscribe
            cbEventMoved(null);
            cbChangedState(null);
            mod.subEventMoved.Subscribe(cbEventMoved);
            mod.subEventChangedState.Subscribe(cbChangedState);
		}
	}

}
