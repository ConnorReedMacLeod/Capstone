using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewArena : Observer {
	public int test;

	public Arena mod;

	public ViewChr [][] arviewChr;

	public GameObject pfChr;

	public void setModel(Arena _mod){
		mod = _mod;
		mod.Subscribe (this);

	}

	public void RegisterChar(Character chr){
		//create a new Character View for the character
		GameObject newObjChr = Instantiate (pfChr, this.transform);
		ViewChr newViewChr = newObjChr.AddComponent<ViewChr> () as ViewChr;

		//Let the view know which character it's representing
		newViewChr.setModel (chr);

		//Add it to the list of Character views
		arviewChr [chr.idOwner] [chr.id] = newViewChr;

	}

	public ViewArena(){
		arviewChr = new ViewChr[Player.MAXPLAYERS][];
		for (int i = 0; i < Player.MAXPLAYERS; i++) {
			arviewChr [i] = new ViewChr[Player.MAXCHRS];
		}
	}
}
