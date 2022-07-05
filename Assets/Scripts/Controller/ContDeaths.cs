using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ContDeaths : Singleton<ContDeaths> {

    public List<Chr> lstDyingChrs;


    public override void Init() {
        lstDyingChrs = new List<Chr>();
    }


    public void AddDyingChr(Chr chr) {

        if(lstDyingChrs.Contains(chr)) {
            Debug.LogFormat("{0} is already in the list of dying characters", chr);
            return;
        }

        lstDyingChrs.Add(chr);
    }

    public void RemoveDyingChr(Chr chr) {

        if(lstDyingChrs.Contains(chr) == false) {
            Debug.LogFormat("{0} are not yet in the list of dying characters", chr);
            return;
        }

        lstDyingChrs.Remove(chr);
    }



}
