using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeUsageChannel : TypeUsage {

    public const int nSkillPointCost = 1;

    public SoulChannel soulBehaviour;
    public int nStartChannelTime;

    public int nSelectionsInputIndex; //Which index we should use from the NetworkReceiver buffer for our input

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

        //Store the index of the current selections so that we can refer back to it later when the channel finishes (or triggers in some way)
        nSelectionsInputIndex = NetworkMatchReceiver.Get().indexCurMatchInput;

        ContSkillEngine.PushSingleClause(new ClauseBeginChannel(skill));
    }

    public override InputSkillSelection GetUsedSelections() {
        return (InputSkillSelection)NetworkMatchReceiver.Get().lstMatchInputBuffer[nSelectionsInputIndex];
    }

    public void ClearStoredSelectionInfo() {
        nSelectionsInputIndex = -1;
    }

    class ClauseBeginChannel : ClauseSkill {

        public ClauseBeginChannel(Skill _skill) : base(_skill) {
        }

        public override string GetDescription() {
            return string.Format("Transition to a channeling state");
        }

        public override void Execute() {

            ContSkillEngine.PushSingleExecutable(new ExecBeginChannel(skill.chrOwner, skill.chrOwner, skill));

        }

    };

}
