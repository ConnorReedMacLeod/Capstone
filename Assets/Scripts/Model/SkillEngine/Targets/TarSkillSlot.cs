using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarSkillSlot : Target {

    public static int SerializeSkillSlot(SkillSlot skillslot) {
        return (TarChr.SerializeChr(skillslot.chrOwner) << 4) + (skillslot.iSlot);
    }

    public static SkillSlot UnserializeSkillSlot(int nSerialized) {
        int iSkillSlot = nSerialized & (15); //select the first 4 bits

        Chr chr = TarChr.UnserializeChr(nSerialized >> 4); //chop off the last 4 bits 

        return chr.arSkillSlots[iSkillSlot];
    }


    public override int Serialize(object objToSerialize) {
        return SerializeSkillSlot((SkillSlot)objToSerialize);
    }

    public override object Unserialize(int nSerialized, List<object> lstSelectionsSoFar) {
        return UnserializeSkillSlot(nSerialized);
    }



    public TarSkillSlot(Skill _skill, FnValidSelection _IsValidSelection) : base(_skill, _IsValidSelection) {

    }

    public override IEnumerable<object> GetSelectableUniverse() {
        List<SkillSlot> lstSkillSlotsSelectable = new List<SkillSlot>();

        foreach(Chr chr in Chr.lstAllChrs) {
            for(int i=0; i<chr.arSkillSlots.Length; i++) {
                lstSkillSlotsSelectable.Add(chr.arSkillSlots[i]);
            }
        }

        return lstSkillSlotsSelectable;
    }


    public static FnValidSelection IsOwnedBySameChr(Chr chr) {
        return (object skillslot, Selections selections) => (chr.id != ((SkillSlot)skillslot).chrOwner.id);
    }

    public static FnValidSelection IsOwnedByOtherChr(Chr chr) {
        return (object skillslot, Selections selections) => (chr.id != ((SkillSlot)skillslot).chrOwner.id);
    }

    public static FnValidSelection IsOwnedBySameTeam(Chr chr) {
        return (object skillslot, Selections selections) => (chr.plyrOwner.id == ((SkillSlot)skillslot).chrOwner.plyrOwner.id);
    }

    public static FnValidSelection IsOwnedByDiffTeam(Chr chr) {
        return (object skillslot, Selections selections) => (chr.plyrOwner.id != ((SkillSlot)skillslot).chrOwner.plyrOwner.id);
    }

    public static FnValidSelection IsOwnedByFrontliner() {
        return (object skillslot, Selections selections) => ((SkillSlot)skillslot).chrOwner.position.positiontype == Position.POSITIONTYPE.FRONTLINE;
    }
    public static FnValidSelection IsOwnedByBackliner() {
        return (object skillslot, Selections selections) => ((SkillSlot)skillslot).chrOwner.position.positiontype == Position.POSITIONTYPE.BACKLINE;
    }


    public override void InitTargetDescription() {
        sTargetDescription = "Select a Character";
    }

    public override void cbClickSelectable(Object target, params object[] args) {
        //Grab the model represented by the view and pass it off to AttemptSelection
        AttemptSelection(((ViewSkill)target).mod.skillslot);
    }

    protected override void OnStartLocalSelection() {

        //TODO:: Figure out how to do good highlighting for valid skillslots
        
        //Set up the character-click triggers
        ViewSkill.subAllClick.Subscribe(cbClickSelectable);
    }

    protected override void OnEndLocalSelection() {
        //TODO:: Unhighlight ALL skillslots

        //Remove the character-click triggers
        ViewSkill.subAllClick.UnSubscribe(cbClickSelectable);
    }
}

