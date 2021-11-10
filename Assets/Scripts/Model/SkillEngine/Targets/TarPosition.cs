using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarPosition : Target {

    public static int SerializePosition(Position pos) {
        return ContPositions.CoordsToIndex(pos.iColumn, pos.jRow);
    }

    public static Position UnserializePosition(int nSerialized) {
        KeyValuePair<int, int> kvpCoords = ContPositions.IndexToCoords(nSerialized);
        return new Position(kvpCoords.Key, kvpCoords.Value);
    }

    public override int Serialize(object objToSerialize) {
        return SerializePosition((Position)objToSerialize);
    }

    public override object Unserialize(int nSerialized, List<object> lstSelectionsSoFar) {
        return UnserializePosition(nSerialized);
    }

    public static TarPosition AddTarget(Skill _skill, FnValidSelection _IsValidSelection) {
        TarPosition tarpos = new TarPosition(_skill, _IsValidSelection);
        _skill.lstTargets.Add(tarpos);

        return tarpos;
    }

    public TarPosition(Skill _skill, FnValidSelection _IsValidSelection) : base(_skill, _IsValidSelection) {

    }

    public override IEnumerable<object> GetSelectableUniverse() {
        return ContPositions.Get().lstAllPositions;
    }

    public static FnValidSelection IsOnPositionOfChr(Chr chr) {
        return (object pos, Selections selections) => (chr.position == (Position)pos);
    }

    public static FnValidSelection IsPositionNotOfChr(Chr chr) {
        return (object pos, Selections selections) => (chr.position != (Position)pos);
    }

    public static FnValidSelection IsSameTeam(Chr chr) {
        return (object pos, Selections selections) => (((Position)pos).IsAllyOwned(chr.plyrOwner));
    }

    public static FnValidSelection IsDiffTeam(Chr chr) {
        return (object pos, Selections selections) => (((Position)pos).IsEnemyOwned(chr.plyrOwner));
    }

    public static FnValidSelection IsFrontliner() {
        return (object pos, Selections selections) => ((Position)pos).positiontype == Position.POSITIONTYPE.FRONTLINE;
    }

    public static FnValidSelection IsBackliner() {
        return (object pos, Selections selections) => ((Position)pos).positiontype == Position.POSITIONTYPE.BACKLINE;
    }

    public static FnValidSelection IsBench() {
        return (object pos, Selections selections) => ((Position)pos).positiontype == Position.POSITIONTYPE.BENCH;
    }


    public override void InitTargetDescription() {
        sTargetDescription = "Select a Position";
    }

    public override void cbClickSelectable(Object target, params object[] args) {
        //Grab the model represented by the view and pass it off to AttemptSelection
        AttemptSelection(((ViewPosition)target).mod);
    }

    protected override void OnStartLocalSelection() {

        //Highlight all the targettable positions
        foreach (Position p in GetValidSelectable(ContLocalUIInteraction.Get().selectionsInProgress)) {
            //Pass along the skill we're trying to select targets for
            p.subBecomesTargettable.NotifyObs(null, ContLocalUIInteraction.Get().selectionsInProgress.skillSelected);
        }

        //Set up the position-click triggers
        ViewPosition.subAllClick.Subscribe(cbClickSelectable);
    }

    protected override void OnEndLocalSelection() {
        //Remove highlighting from ALL Positions (just in case somehow the list of targettable positions may have changed)
        foreach (Position p in GetSelectableUniverse()) {
            p.subEndsTargettable.NotifyObs();
        }

        //Remove the character-click triggers
        ViewPosition.subAllClick.UnSubscribe(cbClickSelectable);
    }
}
