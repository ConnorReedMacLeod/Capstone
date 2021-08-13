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


    public static FnValidSelection IsOtherChr(Chr chr) {
        return (object chr2, Selections selections) => (chr.globalid != ((Chr)chr2).globalid);
    }

    public static FnValidSelection IsSameTeam(Chr chr) {
        return (object chr2, Selections selections) => (chr.plyrOwner.id == ((Chr)chr2).plyrOwner.id);
    }

    public static FnValidSelection IsDiffTeam(Chr chr) {
        return (object chr2, Selections selections) => (chr.plyrOwner.id != ((Chr)chr2).plyrOwner.id);
    }

    public static FnValidSelection IsFrontliner() {
        return (object chr, Selections selections) => ((Chr)chr).position.positiontype == Position.POSITIONTYPE.FRONTLINE;
    }
    public static FnValidSelection IsBackliner() {
        return (object chr, Selections selections) => ((Chr)chr).position.positiontype == Position.POSITIONTYPE.BACKLINE;
    }
}
