using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArgChr : TargetArg {

	public Character chrTar;
	public delegate bool funcLegalChr (Character own, Character arg);
	funcLegalChr fLegalCheck;

	//WARNING: This feels like it should be shared among TargetArgs but it isn't
	public bool setTar(Character _chrTar){
		Character chrOldTar = chrTar;
		chrTar = _chrTar;
		if (VerifyLegal ()) {
			return true; //the targetting was successful
		} else {
			chrTar = chrOldTar;
			return false; //bad target
		}

	}

	public TargetArgChr(funcLegalChr _fLegalCheck){
		fLegalCheck = _fLegalCheck;

	}

	public override bool VerifyLegal(){
		return fLegalCheck (chrOwner, chrTar);
	}

	public override void Reset(){
		chrTar = null;
	}

}
