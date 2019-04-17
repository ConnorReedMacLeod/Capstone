using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArgChr : TargetArg {

  
	public delegate bool funcLegalChr (Chr own, Chr arg);
	public funcLegalChr fLegalCheck;

    public TargetArgChr(funcLegalChr _fLegalCheck){
		fLegalCheck = _fLegalCheck;

	}

    public override bool WouldBeLegal(int indexTarget){

        if(indexTarget >= Chr.arAllChrs.Length) {
            Debug.LogError("Trying to select a character with index " + indexTarget + " that doesn't exist");
            return false;
        }

		return Chr.arAllChrs[indexTarget].bDead == false && fLegalCheck (chrOwner, Chr.arAllChrs[indexTarget]);
	}

}
