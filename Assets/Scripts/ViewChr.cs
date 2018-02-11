using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewChr : Observer {

	//Is this actually overwriting the base mod? Hope so
	public Character mod;
	public Material matChr;

	public ViewArena viewArena; //need to link this up to parent


	const int indexCharBorder = 0;
	const int indexCharPortrait = 1;

	//undoes the scaling of the parent
	public void Unscale(){
		transform.localScale = new Vector3
			(transform.localScale.x / transform.parent.localScale.x,
				transform.localScale.y / transform.parent.localScale.y,
				transform.localScale.z / transform.parent.localScale.z);
	}

	public void setModel(Character _mod){
		mod = _mod;
		mod.Subscribe (this);

		string sMatPath = "Materials/Characters/mat" + mod.sName;
		matChr = Resources.Load(sMatPath, typeof(Material)) as Material;

		GetComponentsInChildren<Renderer> ()[indexCharPortrait].material = matChr;

	}

	override public void UpdateObs(){
		Debug.Log ("I have been Updated");
		// Set position correctly
		transform.localPosition = new Vector3 (mod.fX, mod.fY, 0);
	}

	public void Start(){
		Unscale ();
	}
}
