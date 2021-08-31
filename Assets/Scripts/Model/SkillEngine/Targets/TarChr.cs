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


    public override void InitTargetDescription() {
        sTargetDescription = "Select a Character";
    }

    public override void cbClickSelectable(Object target, params object[] args) {
        //Grab the model represented by the view and pass it off to AttemptSelection
        AttemptSelection(((ViewChr)target).mod);
    }

    public override void OnLocalStartSelection() {
        //Highlight all the targettable characters
        foreach(Chr c in GetValidSelectable(ContLocalUIInteraction.Get().selectionsInProgress)) {
            //Pass along the skill we're trying to select targets for
            c.subBecomesTargettable.NotifyObs(null, ContLocalUIInteraction.Get().selectionsInProgress.skillSelected);
        }

        //Set up the character-click triggers
        ViewChr.subAllClick.Subscribe(cbClickSelectable);
    }

    public override void OnLocalEndSelection() {
        //Remove highlighting from ALL characters (just in case somehow the list of targettable characters may have changed)
        foreach(Chr c in GetSelactableUniverse()) {
            c.subEndsTargettable.NotifyObs();
        }

        //Remove the character-click triggers
        ViewChr.subAllClick.UnSubscribe(cbClickSelectable);
    }
}
