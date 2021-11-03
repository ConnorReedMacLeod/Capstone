using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarChr : Target {

    public static int SerializeChr(Chr chr) {
        return chr.globalid;
    }

    public static Chr UnserializeChr(int nSerialized) {
        return Chr.lstAllChrs[nSerialized];
    }



    public override int Serialize(object objToSerialize) {
        return SerializeChr((Chr)objToSerialize);
    }

    public override object Unserialize(int nSerialized, List<object> lstSelectionsSoFar) {
        return UnserializeChr(nSerialized);
    }

    public static TarChr AddTarget(Skill _skill, FnValidSelection _IsValidSelection) {
        TarChr tarchr = new TarChr(_skill, _IsValidSelection);
        _skill.lstTargets.Add(tarchr);

        return tarchr;
    }

    public TarChr(Skill _skill, FnValidSelection _IsValidSelection) : base(_skill, _IsValidSelection) {

    }

    public override IEnumerable<object> GetSelectableUniverse() {
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

    protected override void OnStartLocalSelection() {

        //Highlight all the targettable characters
        foreach(Chr c in GetValidSelectable(ContLocalUIInteraction.Get().selectionsInProgress)) {
            //Pass along the skill we're trying to select targets for
            c.subBecomesTargettable.NotifyObs(null, ContLocalUIInteraction.Get().selectionsInProgress.skillSelected);
        }

        //Set up the character-click triggers
        ViewChr.subAllClick.Subscribe(cbClickSelectable);
    }

    protected override void OnEndLocalSelection() {
        //Remove highlighting from ALL characters (just in case somehow the list of targettable characters may have changed)
        foreach(Chr c in GetSelectableUniverse()) {
            c.subEndsTargettable.NotifyObs();
        }

        //Remove the character-click triggers
        ViewChr.subAllClick.UnSubscribe(cbClickSelectable);
    }
}
