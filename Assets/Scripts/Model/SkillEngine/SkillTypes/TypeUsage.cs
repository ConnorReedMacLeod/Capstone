using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TypeUsage {

    public Skill skill;
    public enum TYPE { ACTIVE, CANTRIP, CHANNEL, PASSIVE };

    public TypeUsage(Skill _skill) {
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
        Debug.Assert(skill.chrOwner.curStateReadiness.Type() == StateReadiness.TYPE.READY);

        StateReady stateReady = (StateReady)(skill.chrOwner.curStateReadiness);

        //Ensure we have enough Skill Points left to use this skill
        Debug.Assert(stateReady.nCurSkillsLeft >= GetSkillPointCost());

        stateReady.nCurSkillsLeft -= GetSkillPointCost();
    }

    public virtual void UseSkill() {
        //By default, just execute the skill
        skill.Execute();

    }

    //Fetch the current selection information passed to us from the Master
    public virtual InputSkillSelection GetUsedSelections() {

        InputSkillSelection selections = ContSkillSelection.Get().selectionsFromMaster;

        //You can only get legitimate selections for this skill if the selection passed is referring to
        //  this skill
        Debug.Assert(selections != null, "ERROR - Master has passed no selectionsFromMaster at this point");

        Debug.Assert(ContTurns.Get().GetNextActingChr() == skill.chrOwner, "ERROR - The acting character isn't the owner of this skill");

        Debug.Assert(selections.skillslotSelected == skill, "ERROR - The selected skill from the player does not match this skill");

        return selections;

    }

}
