using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateChanneling : StateReadiness {

    public int nChannelTime;

    public SoulChannel soulBehaviour; //Handles all customized behaviour of what the channel effect should do

    public StateChanneling(Chr _chrOwner, int _nChannelTime, SoulChannel _soulBehaviour) : base(_chrOwner) {

        nChannelTime = _nChannelTime;

        //Double check that the soul isn't visible - should just be a hidden implementation
        Debug.Assert(_soulBehaviour.bVisible == false);
        soulBehaviour = _soulBehaviour;

        //Set the channel time to be equal to whatever the soul's duration is

        //Debug.Log("soulBehaviour's skill is initially " + soulBehaviour.skillSource.sName + " with duration " + nChannelTime);
    }

    public override TYPE Type() {
        return TYPE.CHANNELING;
    }


    public override int GetPriority() {
        //The priority of a channeling character should also include the channel time remaining
        return nChannelTime + base.GetPriority();
    }

    //Whenever a potentially invalidating event happens, interrupt this channel if it
    // actually makes the channel targetting invalid
    // this should be subcribed to each potentially invalidating subject
    public void cbInterruptifInvalid(Object target, params object[] args) {

        Debug.Assert(soulBehaviour.skillSource.typeUsage.Type() == TypeUsage.TYPE.CHANNEL);

        //Get the SelectionInfo stored for the channeled skill and check if it is still completable
        if(soulBehaviour.skillSource.CanCompleteAsChannel() == false) {
            //If targetting has become invalid (maybe because someone has died)
            InterruptChannel();

            //Create a new fatigue state to let our character transition to
            StateFatigued newState = new StateFatigued(chrOwner);

            //Transition to the new state
            chrOwner.SetStateReadiness(newState);
        }

    }

    //To be called as part of a stun, before transitioning to the stunned state
    public override void InterruptChannel() {

        Debug.Log("Interupting a skill with " + soulBehaviour.skillSource.sName);

        //Activate any Interruption trigger on the soul effect
        soulBehaviour.OnInterrupted();
    }

    public override void ChangeChanneltime(int _nChange) {
        if(chrOwner.bDead) {
            Debug.Log("Tried to change channeltime, but " + chrOwner.sName + " is dead");
            return;
        }

        //We can actually reduce the channel time if we're in this state

        if(_nChange + nChannelTime < 0) {
            nChannelTime = 0;
        } else {
            nChannelTime += _nChange;
        }

        Debug.Log("Channel time changed to " + nChannelTime);
        //If, for any reason, we've now been put to 0 channeltime, then our channel completes
        // and we transition to the fatigued state
        if(nChannelTime == 0) {

            Debug.Log("Naturally completed the channel, so pushing ExecCompleteChannel");
            ContSkillEngine.Get().AddExec(new ExecCompleteChannel(null, chrOwner));
        }

    }


    public override void Recharge() {
        if(chrOwner.bDead) {
            Debug.Log("Tried to recharge, but " + chrOwner.sName + " is dead");
            return;
        }

        //If we're channeling, instead of reducing fatigue, we only reduce the channel time
        ContSkillEngine.Get().AddExec(new ExecChangeChannel(null, chrOwner, -1));

    }

    public override void OnEnter() {

        chrOwner.soulContainer.ApplySoul(soulBehaviour);

        //TODO:: Add a subscription list of potentially cancelling triggers to listen for a channel
        // which we can then subcribe cbInterrupifInvalid to
        Chr.subAllDeath.Subscribe(cbInterruptifInvalid);

        //Once we're in this state, let people know that the channel time has taken effect
        chrOwner.subChannelTimeChange.NotifyObs();

    }

    public override void OnLeave() {

        //Push a clause onto the stack that will clear the stored selection of the channel we're using
        //  - don't want that stale selection info floating around after it's relevant
        //  - need to do it before removing the soulBehaviour so that the clause gets evaluated after all the
        //    effects of the soulBehaviour have been resolved
        ContSkillEngine.PushSingleClause(new ClauseClearStoredSelection(soulBehaviour.skillSource));

        //TODO:: unsubscribe from all of these cancelling triggers
        Chr.subAllDeath.UnSubscribe(cbInterruptifInvalid);

        chrOwner.soulContainer.RemoveSoul(soulBehaviour);

    }

    class ClauseClearStoredSelection : ClauseSkill {

        public ClauseClearStoredSelection(Skill _skill) : base(_skill) {
        }

        public override string GetDescription() {
            return string.Format("Clear out stored selection info for " + skill.sName);
        }

        public override void Execute() {

            Debug.Log("Pushing ClearStoredSelection for " + skill.sName);
            ContSkillEngine.PushSingleExecutable(new ExecClearStoredSelection(skill.chrOwner, skill.skillslot));

        }

    };

}
