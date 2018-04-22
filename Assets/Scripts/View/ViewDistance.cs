using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewDistance : MonoBehaviour {

	bool bStarted;

	public Transform tfLine;
	public Transform tfDist;
	public TextMesh txtDist;

	public DistanceEndpoint endpointStart;
	public DistanceEndpoint endpointEnd;

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


	public void RenderDistance(){
		if (endpointStart == null || endpointEnd == null)
			return;

		tfLine.localScale = new Vector3 (DistanceEndpoint.Dist (endpointStart, endpointEnd), 0.15f, 1.0f);

		float angle = LibView.GetAngle (endpointStart.GetCenter (), endpointEnd.GetCenter ());

		//this.transform.position = Vector3.zero;
		tfLine.localPosition = new Vector3 
			((endpointStart.GetCenter().x + endpointEnd.GetCenter().x 
				- (Mathf.Cos(Mathf.Deg2Rad * angle)) * (endpointStart.GetRadius() + endpointEnd.GetRadius())
			) / 2, 
			(endpointStart.GetCenter().y + endpointEnd.GetCenter().y
					- (Mathf.Sin(Mathf.Deg2Rad * angle)) * (endpointStart.GetRadius() + endpointEnd.GetRadius())
				) / 2, -0.1f);

		tfLine.localRotation = Quaternion.Euler (0, 0, angle);

		txtDist.text = DistanceEndpoint.Dist (endpointStart, endpointEnd).ToString("F1");
		tfDist.localPosition = new Vector3 (endpointEnd.GetCenter().x, endpointEnd.GetCenter().y, -0.1f);
	}

	public void SetStart(Chr chr){
		endpointStart = new DistanceEndpointChr (chr);

		RenderDistance ();
	}

	public void SetStart(Vector3 v3){
		endpointStart = new DistanceEndpointPos (v3);

		RenderDistance ();
	}

	public void SetEnd(Chr chr){
		endpointEnd = new DistanceEndpointChr (chr);

		RenderDistance ();
	}

	public void SetEnd(Vector3 v3){
		endpointEnd = new DistanceEndpointPos (v3);

		RenderDistance ();
	}


	//Undoes the image and border scaling set by the parent
	public void Unscale(){
		transform.localScale = new Vector3
			(transform.localScale.x / transform.parent.localScale.x,
				transform.localScale.y / transform.parent.localScale.y,
				transform.localScale.z / transform.parent.localScale.z);
	}
		
	// Use this for initialization
	public void Start () {
		if (bStarted == false) {
			bStarted = true;

			Init ();
			Unscale ();
			transform.localPosition = Vector3.zero;

		}
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButton (0)) {
			//For now, always just follow the mouse
			//TODO:: Don't go off the edge (Can have a last valid point vector3)

			ViewChr viewchr = (ViewChr)LibView.IsUnderMouse (typeof(ViewChr));
			if (viewchr != null) {
				SetEnd (viewchr.mod);
			} else {
				SetEnd (LibView.GetMouseLocation ());
			}

		}
	}
}
