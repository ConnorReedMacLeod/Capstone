using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeChannel : TypeSkill {

    public const int nSkillPointCost = 1;

    public SoulChannel soulBehaviour;
    public int nStartChannelTime;
    public SelectionSerializer.SelectionInfo infoStoredSelection;

    public TypeChannel(Skill skill, int _nStartChannelTime, SoulChannel _soulBehaviour) : base(skill) {

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

        //Store a copy of the current SelectionInfo so that we can use it later when the channel finishes (or triggers in some way)
        infoStoredSelection = base.GetSelectionInfo().GetCopy();

        ContSkillEngine.PushSingleClause(new ClauseBeginChannel(skill));
    }

    public override SelectionSerializer.SelectionInfo GetSelectionInfo() {
        //Return the selection info that's been stored
        return infoStoredSelection;
    }

    public void ClearStoredSelectionInfo() {
        infoStoredSelection = null;
    }

    class ClauseBeginChannel : ClauseSpecial {

        public ClauseBeginChannel(Skill _skill) : base(_skill) {
        }

        public override string GetDescription() {
            return string.Format("Transition to a channeling state");
        }

        public override void ClauseEffect() {

            ContSkillEngine.PushSingleExecutable(new ExecBeginChannel(skill.chrSource, skill.chrSource, skill));

        }

    };

}
