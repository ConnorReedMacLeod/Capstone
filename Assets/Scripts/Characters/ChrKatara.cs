using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChrKatara : Character {

	public ChrKatara(int _idOwner, int _id): base(_idOwner, _id){
		sName = "Katara";
		Debug.Log ("I'm " + sName);
	}

}
