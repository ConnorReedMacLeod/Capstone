using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceEndpointPos : DistanceEndpoint {

	public Vector3 v3Focus;

	public DistanceEndpointPos(Vector3 _v3Focus){
		v3Focus = _v3Focus;
	}

	public override Vector3 GetCenter(){
		return v3Focus;
	}
	public override float GetRadius (){
		return 0.0f;
	}
}