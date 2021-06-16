using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TypeSkill {

    public Skill skill;
    public enum TYPE { ACTIVE, CANTRIP, CHANNEL, PASSIVE };

    public TypeSkill(Skill _skill) {
        this.skill = _skill;
    }

    public abstract string getName();
    public abstract TYPE Type();
    public abstract int GetSkillPointCost();

    public virtual bool Usable() {
        //By default, skills can be used

        return true;
    }

    public virtual void PaySkillPoints() {
        //Ensure we're in a ready state
        Debug.Assert(skill.chrSource.curStateReadiness.Type() == StateReadiness.TYPE.READY);

        StateReady stateReady = (StateReady)(skill.chrSource.curStateReadiness);

        //Ensure we have enough Skill Points left to use this skill
        Debug.Assert(stateReady.nCurSkillsLeft >= GetSkillPointCost());

        stateReady.nCurSkillsLeft -= GetSkillPointCost();
    }

    public virtual void UseSkill() {
        //By default, just execute the skill
        skill.Execute();

    }

    //Fetch the current selection information passed to us from the Master
    public virtual SelectionSerializer.SelectionInfo GetSelectionInfo() {

        SelectionSerializer.SelectionInfo infoSelection = ContSkillSelection.Get().infoSelectionFromMaster;

        //You can only get legitimate selection info for this skill if the selection info is referring to
        //  this skill
        Debug.Assert(infoSelection != null, "ERROR - Master has passed no selectionInfo at this point");

        Debug.Assert(ContTurns.Get().GetNextActingChr() == skill.chrSource, "ERROR - The acting character isn't the owner of this skill");

        Debug.Assert(infoSelection.skillUsed == skill, "ERROR - The selected skill from the player does not match this skill");

        return infoSelection;

    }

}
