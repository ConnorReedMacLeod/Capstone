﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LibView {

	// Returns the position of the mouse
	public static Vector3 GetMouseLocation(){
		return Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}

	public static GameObject GetObjectUnderMouse(){
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit)) {
			// If there's a gameobject under the mouse, return it
			return hit.collider.gameObject;
		}

		// Otherwise, return false
		return null;
	}
}
