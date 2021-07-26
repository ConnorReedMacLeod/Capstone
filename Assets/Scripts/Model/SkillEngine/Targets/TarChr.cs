using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarChr : Target {

    public override int Serialize(object objToSerialize) {
        return ((Chr)objToSerialize).globalid;
    }

    public override object Unserialize(int nSerialized) {
        return Chr.lstAllChrs[nSerialized];
    }


    public TarChr(FnValidSelection _IsValidSelection) : base(_IsValidSelection) {

    }

    public override IEnumerable<object> GetSelactableUniverse() {

        return Chr.lstAllChrs;
    }

}
