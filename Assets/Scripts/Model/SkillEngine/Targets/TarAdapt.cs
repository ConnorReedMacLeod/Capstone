using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TarAdapt : Target {

    public int iTargetSkillSlotToReplace;

    public override int Serialize(object objToSerialize) {

        //Just convert the SkillType we've been passed along (converting into an int)

        return (int)objToSerialize;
    }

    public override object Unserialize(int nSerialized, List<object> lstSelectionsSoFar) {

        //Just convert the int back into the corresponding skill type

        return (SkillType.SKILLTYPE)nSerialized;
    }


    public static TarAdapt AddTarget(Skill _skill, FnValidSelection _IsValidSkillSlot, FnValidSelection _IsValidAdaptSkill) {

        TarSkillSlot tarskillslot = TarSkillSlot.AddTarget(_skill, _IsValidSkillSlot);

        TarAdapt tarAdapt = new TarAdapt(_skill, _IsValidAdaptSkill);

        _skill.lstTargets.Add(tarAdapt);

        tarAdapt.iTargetSkillSlotToReplace = tarskillslot.iTargetIndex;

        return tarAdapt;
    }

    //There's currently nothing extra to configure our TarAdapt with - can always introduce new things here if needed
    public TarAdapt(Skill _skill, FnValidSelection _fnValidSelection) : base(_skill, _fnValidSelection) {

    }

    //This doesn't really make sense for this targetting type, so just return an empty list
    public override IEnumerable<object> GetSelectableUniverse() {
        return null;
    }

    public SkillSlot GetSelectedSkillslot() {
        return (SkillSlot)(selectionsSoFar.lstSelections[iTargetSkillSlotToReplace]);
    }

    public List<SkillType.SkillTypeInfo> GetAdaptableSkills() {
        //Get the skillslot we're adapting away from
        SkillSlot skillslotAdaptingAwayFrom = GetSelectedSkillslot();
        Chr chrAdapting = skillslotAdaptingAwayFrom.chrOwner;

        //Get all the skills under this character's disciplines
        List<SkillType.SkillTypeInfo> lstSkillTypeInfosInDisciplines = SkillType.GetSkillInfosUnderDisciplines(chrAdapting).ToList();

        //Filter out any non-viable ones (e.g., ones we already have equipped in that characters slots
        FilterUnadaptableSkills(chrAdapting, ref lstSkillTypeInfosInDisciplines);

        //return the filtered list
        return lstSkillTypeInfosInDisciplines;
    }

    public void FilterUnadaptableSkills(Chr chrAdapting, ref List<SkillType.SkillTypeInfo> lstPossiblyAdaptableSkills) {

        //For each skill that we can adapt into, remove it if it's already equipped by the character
        lstPossiblyAdaptableSkills.RemoveAll((skilltypeinfo) => chrAdapting.HasSkillEquipped(skilltypeinfo.type));
    }

    public override object GetRandomSelectable() {

        return LibRandom.GetRandomElementOfList<SkillType.SkillTypeInfo>(GetAdaptableSkills()).type;
    }

    public override void InitTargetDescription() {
        sTargetDescription = "Select the skill to adapt into";
    }

    public override string GetHistoryDescription(object objTarget) {
        return "??? skill";
    }


    //Hooked up to the 'submit' button/trigger for after the mana payments have been selected
    public override void cbClickSelectable(Object target, params object[] args) {
        //Pass along the built-up mana selection 
        AttemptSelection(target);
    }

    protected override void OnStartLocalSelection() {

        //Let the Adapting panel know that we need to adapt into a new skill and let it 
        // know which skills it should be offering as options
        ViewTarAdapt.Get().StartSelection(this);

    }

    protected override void OnEndLocalSelection() {

        //Now that we're done paying, have the ViewTarAdapt clean itself up
        ViewTarAdapt.Get().CleanUp();
    }
}

