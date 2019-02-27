using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArgPos : TargetArg {

	public Vector3 v3Tar;
	public delegate bool funcLegalPos (Chr own, Vector3 v3Tar);
	funcLegalPos fLegalCheck;

    public override void SetTarget(int indexTarget) {
        throw new System.NotImplementedException();
    }

    public TargetArgPos(funcLegalPos _fLegalCheck){
		fLegalCheck = _fLegalCheck;

	}

    public override bool CurrentlyLegal() {
        throw new System.NotImplementedException();
    }

    public override bool VerifyLegal(int indexTarget){
		return fLegalCheck (chrOwner, v3Tar);
	}

	public override void Reset(){
		//Note that this isn't really a sentinel value like null - problem?
		v3Tar = Vector3.zero;
	}

}
