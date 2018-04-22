using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DistanceEndpoint {

	public abstract Vector3 GetCenter();
	public abstract float GetRadius ();

	public static float Dist(DistanceEndpoint dist1, DistanceEndpoint dist2){
		
		return (Vector3.Distance (dist1.GetCenter (), dist2.GetCenter ()) - dist1.GetRadius ()) - dist2.GetRadius ();
	}
}
