using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarPosition : Target {

    public override int Serialize(object objToSerialize) {
        return Serializer.SerializeByte((Position)objToSerialize);
    }

    public override object Unserialize(int nSerialized, List<object> lstSelectionsSoFar) {
        return Serializer.DeserializePosition(nSerialized);
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
        return (object pos, InputSkillSelection selections) => (chr.position == (Position)pos);
    }

    public static FnValidSelection IsPositionNotOfChr(Chr chr) {
        return (object pos, InputSkillSelection selections) => (chr.position != (Position)pos);
    }

    public static FnValidSelection IsEmptyPosition() {
        return (object pos, InputSkillSelection selections) => (((Position)pos).chrOnPosition == null);
    }

    public static FnValidSelection IsOccupiedPosition() {
        return (object pos, InputSkillSelection selections) => (((Position)pos).chrOnPosition != null);
    }

    public static FnValidSelection IsSameTeam(Chr chr) {
        return (object pos, InputSkillSelection selections) => (((Position)pos).IsAllyOwned(chr.plyrOwner));
    }

    public static FnValidSelection IsDiffTeam(Chr chr) {
        return (object pos, InputSkillSelection selections) => (((Position)pos).IsEnemyOwned(chr.plyrOwner));
    }

    public static FnValidSelection IsFrontline() {
        return (object pos, InputSkillSelection selections) => ((Position)pos).positiontype == Position.POSITIONTYPE.FRONTLINE;
    }

    public static FnValidSelection IsBackline() {
        return (object pos, InputSkillSelection selections) => ((Position)pos).positiontype == Position.POSITIONTYPE.BACKLINE;
    }

    public static FnValidSelection IsBench() {
        return (object pos, InputSkillSelection selections) => ((Position)pos).positiontype == Position.POSITIONTYPE.BENCH;
    }


    public override void InitTargetDescription() {
        sTargetDescription = "Select a Position";
    }

    public override string GetHistoryDescription(object objTarget) {
        Position posSelected = (Position)objTarget;

        //Add highlighting depending on if the position was allied or not
        return LibText.AddAllegianceColour(posSelected.ToPrettyString(), skill.chrOwner.plyrOwner.id == posSelected.PlyrIdOwnedBy());
    }

    public override void cbClickSelectable(Object target, params object[] args) {
        //Grab the model represented by the view and pass it off to AttemptSelection
        AttemptSelection(((ViewPosition)target).mod);
    }

    //Note - pretty much just copied from TarChr
    protected override void ShiftCameraToTarget() {

        bool bHasActivePlyr0 = false;
        bool bHasActivePlyr1 = false;
        bool bHasBenchPlyr0 = false;
        bool bHasBenchPlyr1 = false;

        //Cycle through  each selectable position and record what types of targets we see
        foreach(Position p in GetValidSelectable(ContLocalUIInteraction.Get().selectionsInProgress)) {

            if(p.PlyrIdOwnedBy() == 0) {
                if(p.positiontype == Position.POSITIONTYPE.BENCH) {
                    bHasBenchPlyr0 = true;
                } else {
                    bHasActivePlyr0 = true;
                }
            } else if(p.PlyrIdOwnedBy() == 1) {
                if(p.positiontype == Position.POSITIONTYPE.BENCH) {
                    bHasBenchPlyr1 = true;
                } else {
                    bHasActivePlyr1 = true;
                }
            }
        }

        string sCameraLocation = "Home";

        if(bHasBenchPlyr0 && bHasBenchPlyr1) {
            //If both players' benches have selectable positions, then we should zoom out to show both
            sCameraLocation = "ZoomedOut";
        } else if(bHasBenchPlyr0) {
            //If we only need to look at player 0's bench, shift to the left side
            sCameraLocation = "BenchLeft";
        } else if(bHasBenchPlyr1) {
            //If we only need to look at player 1's bench, shift to the right side
            sCameraLocation = "BenchRight";
        } else {
            //Otherwise, just head to the center position
            sCameraLocation = "Home";
        }

        //Send along the camera location we've chosen to the camera controller
        Match.Get().cameraControllerMatch.SetTargetLocation(sCameraLocation);
    }

    protected override void OnStartLocalSelection() {

        //Highlight all the targettable positions
        foreach(Position p in GetValidSelectable(ContLocalUIInteraction.Get().selectionsInProgress)) {
            //Pass along the skill we're trying to select targets for
            Debug.Log("About to let " + p + " know that it is selectable with sub " + p.subBecomesTargettable);
            Debug.Log("Selecting for " + ContLocalUIInteraction.Get().selectionsInProgress.skillslotSelected);
            p.subBecomesTargettable.NotifyObs(null, ContLocalUIInteraction.Get().selectionsInProgress.skillslotSelected);
        }

        //Set up the position-click triggers
        ViewPosition.subAllClick.Subscribe(cbClickSelectable);
    }

    protected override void OnEndLocalSelection() {
        //Remove highlighting from ALL Positions (just in case somehow the list of targettable positions may have changed)
        foreach(Position p in GetSelectableUniverse()) {
            p.subEndsTargettable.NotifyObs();
        }

        //Remove the character-click triggers
        ViewPosition.subAllClick.UnSubscribe(cbClickSelectable);
    }
}
