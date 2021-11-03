using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeUsageChannel : TypeUsage {

    public const int nSkillPointCost = 1;

    public SoulChannel soulBehaviour;
    public int nStartChannelTime;
    public Selections selectionsStored;

    public TypeUsageChannel(Skill skill, int _nStartChannelTime, SoulChannel _soulBehaviour) : base(skill) {

        nStartChannelTime = _nStartChannelTime;

        //If we've been given special soul effect, then use it
        if(_soulBehaviour != null) {
            soulBehaviour = _soulBehaviour;
        } else {
            //Otherwise just make a blank one
            Debug.Log("Warning - making a blank channel soul behaviour");
            soulBehaviour = new SoulChannel(skill);

            //Since this is a specially created soulBehaviour, we don't need to
            //  do anything other than call the skill's Execute function when we complete channeling
            soulBehaviour.bDelayedSkill = true;
        }

        //Note that the soulbehaviour will be invisible and have infinite duration - it will just be removed
        //  by some non-time related trigger (typically the character transitioning away from a Channeling State)
    }

    public override string getName() {
        return "Channel";
    }

    public override TYPE Type() {
        return TYPE.CHANNEL;
    }
    public override int GetSkillPointCost() {
        return nSkillPointCost;
    }

    public override void UseSkill() {

        //Store a copy of the current selections so that we can use it later when the channel finishes (or triggers in some way)
        selectionsStored = base.GetUsedSelections().GetCopy();

        ContSkillEngine.PushSingleClause(new ClauseBeginChannel(skill));
    }

    public override Selections GetUsedSelections() {
        return selectionsStored;
    }

    public void ClearStoredSelectionInfo() {
        selectionsStored = null;
    }

    class ClauseBeginChannel : Clause {

        public ClauseBeginChannel(Skill _skill) : base(_skill) {
        }

        public override string GetDescription() {
            return string.Format("Transition to a channeling state");
        }

        public override void ClauseEffect(Selections selections) {

            ContSkillEngine.PushSingleExecutable(new ExecBeginChannel(skill.chrOwner, skill.chrOwner, skill));

        }

    };

}
