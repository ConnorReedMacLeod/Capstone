using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseChrTargetter : BaseTargetter<Chr> {

    public enum BASECHRTARGETTERTYPE { SELF, MELEE, RANGED, SWEEPING };
    public Property<BASECHRTARGETTERTYPE> pBaseChrTargetterType;

    BASECHRTARGETTERTYPE baseChrTargetterTypeInit;
    
    public BaseChrTargetter(BASECHRTARGETTERTYPE _baseChrTargetterTypeInit) {
        baseChrTargetterTypeInit = _baseChrTargetterTypeInit;

        //Initiallize the modifiable property's base value to be the Init
        pBaseChrTargetterType = new Property<BASECHRTARGETTERTYPE>(baseChrTargetterTypeInit);
    }

    public override List<Chr> GetBaseTargets() {

        //TODONOW:: Consider if the original init type being different from the 
        //       resulting type actually matters - likely not if we just account
        //       for the possibility that the stored serialized selection data may
        //       not be valid

        switch (pBaseChrTargetterType.Get()) {

            //TODONOW:: Fix these return types to check stored serialized values
            case BASECHRTARGETTERTYPE.SELF:

                return null;

            case BASECHRTARGETTERTYPE.MELEE:

                return null;

            case BASECHRTARGETTERTYPE.RANGED:

                return null;

            case BASECHRTARGETTERTYPE.SWEEPING:

                return null;

            default:
                Debug.LogError("ERROR! Unrecognized BaseChrTargettingType");
                return null;
        }

    }
}
