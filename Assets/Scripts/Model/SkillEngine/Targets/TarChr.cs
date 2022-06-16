using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarChr : Target {

    public static int SerializeChr(Chr chr) {
        return chr.id;
    }

    public static Chr UnserializeChr(int nSerialized) {
        return ChrCollection.Get().GetChr(nSerialized);
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

    public override bool CanSelect(object objSelected, InputSkillSelection selectionsSoFar) {
        //Ask the character if they need to override the basic selection validation that we'd normally do in our TarChr checks
        bool bCanSelect = ((Chr)objSelected).pOverrideCanBeSelectedBy.Get()(this, selectionsSoFar, base.CanSelect(objSelected, selectionsSoFar));

        return bCanSelect;
    }

    public override IEnumerable<object> GetSelectableUniverse() {
        return ChrCollection.Get().GetAllLiveChrs();
    }


    public static FnValidSelection IsOtherChr(Chr chr) {
        return (object chr2, InputSkillSelection selections) => (chr.id != ((Chr)chr2).id);
    }

    public static FnValidSelection IsSameTeam(Chr chr) {
        return (object chr2, InputSkillSelection selections) => (chr.plyrOwner.id == ((Chr)chr2).plyrOwner.id);
    }

    public static FnValidSelection IsDiffTeam(Chr chr) {
        return (object chr2, InputSkillSelection selections) => (chr.plyrOwner.id != ((Chr)chr2).plyrOwner.id);
    }

    public static FnValidSelection IsFrontliner() {
        return (object chr, InputSkillSelection selections) => ((Chr)chr).position.positiontype == Position.POSITIONTYPE.FRONTLINE;
    }
    public static FnValidSelection IsBackliner() {
        return (object chr, InputSkillSelection selections) => ((Chr)chr).position.positiontype == Position.POSITIONTYPE.BACKLINE;
    }
    public static FnValidSelection IsInPlay() {
        return (object chr, InputSkillSelection selections) => ((Chr)chr).position.positiontype != Position.POSITIONTYPE.BENCH;
    }
    public static FnValidSelection IsBenched() {
        return (object chr, InputSkillSelection selections) => ((Chr)chr).position.positiontype == Position.POSITIONTYPE.BENCH;
    }


    public override void InitTargetDescription() {
        sTargetDescription = "Select a Character";
    }

    public override string GetHistoryDescription(object objTarget) {
        Chr chrSelected = (Chr)objTarget;

        //Set the highlighting to be either green or red depending on if the target is on the same team or not
        return LibText.AddAllegianceColour(chrSelected.sName, skill.chrOwner.plyrOwner.id == chrSelected.plyrOwner.id);

    }

    public override void cbClickSelectable(Object target, params object[] args) {
        //Grab the model represented by the view and pass it off to AttemptSelection
        AttemptSelection(((ViewChr)target).mod);
    }

    protected override void ShiftCameraToTarget() {

        bool bHasActivePlyr0 = false;
        bool bHasActivePlyr1 = false;
        bool bHasBenchPlyr0 = false;
        bool bHasBenchPlyr1 = false;

        //Cycle through  each selectable character and record what types of targets we see
        foreach(Chr c in GetValidSelectable(ContLocalUIInteraction.Get().selectionsInProgress)) {

            if(c.plyrOwner.id == 0) {
                if(c.position.positiontype == Position.POSITIONTYPE.BENCH) {
                    bHasBenchPlyr0 = true;
                } else {
                    bHasActivePlyr0 = true;
                }
            } else if(c.plyrOwner.id == 1) {
                if(c.position.positiontype == Position.POSITIONTYPE.BENCH) {
                    bHasBenchPlyr1 = true;
                } else {
                    bHasActivePlyr1 = true;
                }
            }
        }

        string sCameraLocation = "Home";

        if(bHasBenchPlyr0 && bHasBenchPlyr1) {
            //If both players' benches have selectable characters, then we should zoom out to show both
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

        //Highlight all the targettable characters
        foreach(Chr c in GetValidSelectable(ContLocalUIInteraction.Get().selectionsInProgress)) {
            //Pass along the skill we're trying to select targets for
            c.subBecomesTargettable.NotifyObs(null, ContLocalUIInteraction.Get().selectionsInProgress.skillslotSelected);
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

    //Performs some default checks for if a given selected character for a channel's target is still
    //  legal enough to let the channel complete.  
    public virtual bool DefaultCanCompleteAsChannelTarget(Chr chr, InputSkillSelection selectionsStored) {
        //By default, just check if the target character is still alive and in-play
        if(chr.bDead == true) {
            Debug.Log("Can't complete a channel selecting a dead character");
            return false;
        } else if(chr.position.positiontype != Position.POSITIONTYPE.BENCH) {
            Debug.Log("Can't complete a channel selecting a benched character");
            return false;
        } else if(chr.pOverrideCanBeSelectedBy.Get()(this, selectionsStored, true) == false) {
            Debug.Log("Can't complete a channel since it's selection overrides have denied this character from being legally targettable");
            return false;
        }
        //TODO - consider if this should also confirm that the team the character is on hasn't changed

        return false;
    }

    //Gets the list of triggers associated with the default checks we should do to ensure 
    //  that the targetted character is still legal enough of a target to complete a channel
    // Note - should be paired with the checks in DefaultCanCompleteAsChannelTarget
    public virtual void AddDefaultTriggersToCompleteAsChannel(List<Subject> lstTriggersSoFar, Chr chr) {

        lstTriggersSoFar.Add(chr.subDeath);
        lstTriggersSoFar.Add(chr.subEnteredBench);

    }
}
