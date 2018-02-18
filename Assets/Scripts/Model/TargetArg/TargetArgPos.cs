using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArgPos : TargetArg {

	public Vector3 v3Tar;
	public delegate bool funcLegalPos (Character own, Vector3 v3Tar);
	funcLegalPos fLegalCheck;

	//WARNING: This feels like it should be shared among TargetArgs but it isn't
	public bool setTar(Vector3 _v3Tar){
		Vector3 chrOldTar = v3Tar;
		v3Tar = _v3Tar;
		if (VerifyLegal ()) {
			return true; //the targetting was successful
		} else {
			v3Tar = chrOldTar;
			return false; //bad target
		}

	}

	public TargetArgPos(funcLegalPos _fLegalCheck){
		fLegalCheck = _fLegalCheck;

	}

	public override bool VerifyLegal(){
		return fLegalCheck (chrOwner, v3Tar);
	}

	public override void Reset(){
		//Note that this isn't really a sentinel value like null - problem?
		v3Tar = Vector3.zero;
	}

}
