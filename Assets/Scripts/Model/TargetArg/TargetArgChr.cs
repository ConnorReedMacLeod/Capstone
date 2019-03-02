using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArgChr : TargetArg {

  
	public Chr chrTar;
	public delegate bool funcLegalChr (Chr own, Chr arg);
	public funcLegalChr fLegalCheck;


    public override void SetTarget(int indexTarget) {
        chrTar = Chr.arAllChrs[indexTarget];
    }

    public TargetArgChr(funcLegalChr _fLegalCheck){
		fLegalCheck = _fLegalCheck;

	}

    public override bool CurrentlyLegal() {
        return chrTar.bDead == false && chrTar != null && fLegalCheck(chrOwner, chrTar);
    }

    public override bool WouldBeLegal(int indexTarget){
		return chrTar.bDead == false && fLegalCheck (chrOwner, Chr.arAllChrs[indexTarget]);
	}

	public override void Reset(){
		chrTar = null;
	}

}
