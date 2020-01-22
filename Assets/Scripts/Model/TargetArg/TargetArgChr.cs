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

        if(indexTarget >= Chr.lstAllChrs.Count) {
            Debug.LogError("Trying to select a character with index " + indexTarget + " that doesn't exist");
            return false;
        }

		return Chr.lstAllChrs[indexTarget].bDead == false && fLegalCheck (chrOwner, Chr.lstAllChrs[indexTarget]);
	}

}
