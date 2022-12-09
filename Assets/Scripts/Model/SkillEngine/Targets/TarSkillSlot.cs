using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarSkillSlot : Target {

    public override int Serialize(object objToSerialize) {
        return Serializer.SerializeByte((SkillSlot)objToSerialize);
    }

    public override object Unserialize(int nSerialized, List<object> lstSelectionsSoFar) {
        return Serializer.DeserializeSkillSlot(nSerialized);
    }

    public static TarSkillSlot AddTarget(Skill _skill, FnValidSelection _IsValidSelection) {
        TarSkillSlot tarskillslot = new TarSkillSlot(_skill, _IsValidSelection);
        _skill.lstTargets.Add(tarskillslot);

        return tarskillslot;
    }

    public TarSkillSlot(Skill _skill, FnValidSelection _IsValidSelection) : base(_skill, _IsValidSelection) {

    }

    public override IEnumerable<object> GetSelectableUniverse() {
        List<SkillSlot> lstSkillSlotsSelectable = new List<SkillSlot>();

        foreach(Chr chr in ChrCollection.Get().GetAllActiveChrs()) {
            for(int i = 0; i < chr.arSkillSlots.Length; i++) {
                lstSkillSlotsSelectable.Add(chr.arSkillSlots[i]);
            }
        }

        return lstSkillSlotsSelectable;
    }


    public static FnValidSelection IsOwnedBySameChr(Chr chr) {
        return (object skillslot, InputSkillSelection selections) => (chr.id == ((SkillSlot)skillslot).chrOwner.id);
    }

    public static FnValidSelection IsOwnedByOtherChr(Chr chr) {
        return (object skillslot, InputSkillSelection selections) => (chr.id != ((SkillSlot)skillslot).chrOwner.id);
    }

    public static FnValidSelection IsOwnedBySameTeam(Chr chr) {
        return (object skillslot, InputSkillSelection selections) => (chr.plyrOwner.id == ((SkillSlot)skillslot).chrOwner.plyrOwner.id);
    }

    public static FnValidSelection IsOwnedByDiffTeam(Chr chr) {
        return (object skillslot, InputSkillSelection selections) => (chr.plyrOwner.id != ((SkillSlot)skillslot).chrOwner.plyrOwner.id);
    }

    public static FnValidSelection IsOwnedByFrontliner() {
        return (object skillslot, InputSkillSelection selections) => ((SkillSlot)skillslot).chrOwner.position.positiontype == Position.POSITIONTYPE.FRONTLINE;
    }
    public static FnValidSelection IsOwnedByBackliner() {
        return (object skillslot, InputSkillSelection selections) => ((SkillSlot)skillslot).chrOwner.position.positiontype == Position.POSITIONTYPE.BACKLINE;
    }


    public override void InitTargetDescription() {
        sTargetDescription = "Select a Character";
    }

    public override string GetHistoryDescription(object objTarget) {
        SkillSlot ssSelected = (SkillSlot)objTarget;

        //Set the highlighting to be either green or red depending on if the target is on the same team or not
        return LibText.AddAllegianceColour(ssSelected.ToString(), skill.chrOwner.plyrOwner.id == ssSelected.chrOwner.plyrOwner.id);
    }


    public override void cbClickSelectable(Object target, params object[] args) {
        //Grab the model represented by the view and pass it off to AttemptSelection
        AttemptSelection(((ViewSkill)target).mod.skillslot);
    }

    // Mostly copied from TarChr's implementation
    protected override void ShiftCameraToTarget() {

        bool bHasActivePlyr0 = false;
        bool bHasActivePlyr1 = false;
        bool bHasBenchPlyr0 = false;
        bool bHasBenchPlyr1 = false;

        //Cycle through  each selectable character and record what types of targets we see
        foreach(SkillSlot ss in GetValidSelectable(ContLocalUIInteraction.Get().selectionsInProgress)) {

            if(ss.chrOwner.plyrOwner.id == 0) {
                if(ss.chrOwner.position.positiontype == Position.POSITIONTYPE.BENCH) {
                    bHasBenchPlyr0 = true;
                } else {
                    bHasActivePlyr0 = true;
                }
            } else if(ss.chrOwner.plyrOwner.id == 1) {
                if(ss.chrOwner.position.positiontype == Position.POSITIONTYPE.BENCH) {
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

        //TODO:: Figure out how to do good highlighting for valid skillslots

        //Allow the local player to click on characters to view and select their skills
        ContLocalUIInteraction.Get().bCanSelectCharacters = true;

        //Set up the character-click triggers
        ViewSkill.subAllClick.Subscribe(cbClickSelectable);
    }

    protected override void OnEndLocalSelection() {
        //TODO:: Unhighlight ALL skillslots

        //Remove the character-click triggers
        ViewSkill.subAllClick.UnSubscribe(cbClickSelectable);

        //Set our local selection state to idle since we don't want the character's skills window to still be up after clicking one
        ContLocalUIInteraction.Get().SetState(new StateTargetIdle());
    }
}

