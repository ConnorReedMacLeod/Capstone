using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : Element {

	public ViewArena viewArena;
	public ViewTimeline viewTimeline;
		
	public void Start(){
		viewArena = GetComponentInChildren<ViewArena> ();
		viewTimeline = GetComponentInChildren<ViewTimeline> ();
	}
}
