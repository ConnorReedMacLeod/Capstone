using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TarAdapt : Target {

    public override int Serialize(object objToSerialize) {
        
        //Just convert the SkillType we've been passed along (converting into an int)

        return (int)objToSerialize;
    }

    public override object Unserialize(int nSerialized, List<object> lstSelectionsSoFar) {

        //Just convert the int back into the corresponding skill type

        return (SkillType.SKILLTYPE)nSerialized;
    }
    

    //There's currently nothing extra to configure our TarAdapt with - can always introduce new things here if needed
    public TarAdapt(Skill _skill, FnValidSelection _fnValidSelection) : base(_skill, _fnValidSelection) {

    }

    //This doesn't really make sense for this targetting type, so just return an empty list
    public override IEnumerable<object> GetSelectableUniverse() {
        return null;
    }

    public List<SkillType.SkillTypeInfo> GetAdaptableSkills() {
        //We assume the most recent selection was the skill we're adapting away from
        Chr chrAdapting = ((Skill)selectionsSoFar.GetNthPreviousSelection(0)).chrOwner;

        //Get all the skills under this character's disciplines
        List<SkillType.SkillTypeInfo> lstSkillTypeInfosInDisciplines = SkillType.GetSkillInfosUnderDisciplines(chrAdapting).ToList();

        //Filter out any non-viable ones (e.g., ones we already have equipped in that characters slots
        //todonow
        //return the filtered list
        return lstSkillTypeInfosInDisciplines;
    }

    public override object GetRandomSelectable() {

        return LibRandom.GetRandomElementOfList<SkillType.SkillTypeInfo>(GetAdaptableSkills()).type;
    }

    public override void InitTargetDescription() {
        sTargetDescription = "Select the skill to adapt into";
    }


    //Hooked up to the 'submit' button/trigger for after the mana payments have been selected
    public override void cbClickSelectable(Object target, params object[] args) {
        //Pass along the built-up mana selection 
        AttemptSelection(target);
    }

    protected override void OnStartLocalSelection() {

        //Bring out the ViewTarAdapt and initialize it to be offering selections to adapt to
        GameObject pfAdaptPanel = Resources.Load("Prefabs/TargettingUI/pfAdaptPanel", typeof(GameObject)) as GameObject;
        ViewTarAdapt viewTarAdapt = pfAdaptPanel.GetComponent<ViewTarAdapt>();
        viewTarAdapt.StartSelection(this);

    }

    protected override void OnEndLocalSelection() {

        //Now that we're done paying, have the ViewTarAdapt clean itself up
        ViewTarMana.Get().CleanUp();

    }
}

