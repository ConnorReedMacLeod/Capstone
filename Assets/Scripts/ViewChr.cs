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

	public void setModel(Character _mod){
		mod = _mod;
		mod.Subscribe (this);

		string sMatPath = "Materials/Characters/mat" + mod.sName;
		matChr = Resources.Load(sMatPath, typeof(Material)) as Material;

		GetComponentsInChildren<Renderer> ()[indexCharPortrait].material = matChr;

	}
}
