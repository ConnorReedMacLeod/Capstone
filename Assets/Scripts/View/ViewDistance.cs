using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewDistance : MonoBehaviour {

	bool bStarted;

	public Transform tfLine;
	public Transform tfDist;
	public TextMesh txtDist;

	public Vector3 v3Start;
	public Vector3 v3End;

	public float fMinDist;

	public void Init(){

		txtDist = GetComponentInChildren<TextMesh> ();
		if (txtDist == null) {
			Debug.LogError ("ERROR! NO TEXTMESH CHILD OF VIEWDISTANCE");
		}

		Transform[] artf = GetComponentsInChildren<Transform> ();
		foreach (Transform tf in artf) {
			switch (tf.name) {
			case "objLine": 
				tfLine = tf;
				break;

			case "txtDist":
				tfDist = tf;
				break;

			default:
				
				break;

			}
		}

		if (tfLine == null) {
			Debug.LogError ("ERROR! NO LINETRANSFORM CHILD OF VIEWDISTANCE");
		}

		if (tfDist == null) {
			Debug.LogError ("ERROR! NO TXTTRANSFORM CHILD OF VIEWDISTANCE");
		}

	}

	public static float Dist(Vector3 v1, Vector3 v2){
		float fDeltaX = v1.x - v2.x;
		float fDeltaY = v1.y - v2.y;
		return Mathf.Sqrt (Mathf.Pow (fDeltaX, 2) + Mathf.Pow (fDeltaY, 2));
	}

	public void RenderDistance(){
		Debug.Log (Dist (v3Start, v3End) + " is dist");
		tfDist.localScale = new Vector3 (Dist (v3Start, v3End), 0.15f, 1.0f);

		this.transform.localPosition = new Vector3 ((v3End.x - v3Start.x) / 2, (v3End.y - v3Start.y), 0);
	}

	public void SetStart(Vector3 _v3Start){
		v3Start = _v3Start;

		if (v3End != null) {
			RenderDistance ();
		}
	}

	public void SetEnd(Vector3 _v3End){
		v3End = _v3End;

		if (v3Start != null) {
			RenderDistance ();
		}
	}

	// Use this for initialization
	public void Start () {
		if (bStarted == false) {
			bStarted = true;

			Init ();
			fMinDist = 1.0f;
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButton (0)) {
			//For now, always just follow the mouse
			//TODO:: Latch on to Chrs
			//TODO:: Don't go off the edge (Can have a last valid point vector3)
			SetEnd(ViewArena.GetArenaPos(Input.mousePosition));
		}
	}
}
