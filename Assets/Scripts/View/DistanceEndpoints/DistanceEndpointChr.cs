using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceEndpointChr : DistanceEndpoint {

	public Chr chrFocus;

	public DistanceEndpointChr(Chr _chrFocus){
		chrFocus = _chrFocus;
	}

	public override Vector3 GetCenter(){
		return chrFocus.v3Pos;
	}
	public override float GetRadius (){
		return chrFocus.fRad;
	}
}
