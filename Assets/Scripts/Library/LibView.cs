using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LibView {

	// Returns the position of the mouse
	public static Vector3 GetMouseLocation(){
		return Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}
}
