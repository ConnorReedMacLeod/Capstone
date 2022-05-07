using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateChanneling : StateReadiness {

    public int nChannelTime;

    public SoulChannel soulBehaviour; //Handles all customized behaviour of what the channel effect should do

    //TODO - Consider if this should be implemented as a list of pairs of Subject, Predicate pairs that only
    //       check the relevent cancellation condition when the subject is updated.  Could save some computation time, 
    //       but might not be as coder-friendly to set up skills for
    public List<Subject> lstPotentialChannelCancelTriggers; //Can define a list of subjects we'll subscribe ourselves to that 
                                                            // are points at which we need to confirm if we should cancel channeling or not

    public StateChanneling(Chr _chrOwner, int _nChannelTime, SoulChannel _soulBehaviour, List<Subject> _lstPotentialChannelCancelTriggers) : base(_chrOwner) {

        nChannelTime = _nChannelTime;

        //Double check that the soul isn't visible - should just be a hidden implementation
        Debug.Assert(_soulBehaviour.bVisible == false);
        soulBehaviour = _soulBehaviour;

        lstPotentialChannelCancelTriggers = _lstPotentialChannelCancelTriggers;

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

        Debug.Log("Interupting the channel, " + soulBehaviour.skillSource.sName);

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

        if (lstPotentialChannelCancelTriggers == null) {
            Debug.LogError("ERROR! Must provide a list of potential channel cancellation triggers! (Could be empty if needed)");
            return;
        }

        //Add in any baseline potential cancellation triggers
        lstPotentialChannelCancelTriggers.Add(Chr.subAllDeath);

        //Subscribe to each potential cancellation trigger 
        for(int i=0; i<lstPotentialChannelCancelTriggers.Count; i++) {
            lstPotentialChannelCancelTriggers[i].Subscribe(cbInterruptifInvalid);
        }

        //Once we're in this state, let people know that the channel time has taken effect
        chrOwner.subChannelTimeChange.NotifyObs();

    }

    public override void OnLeave() {

        //Push a clause onto the stack that will clear the stored selection of the channel we're using
        //  - don't want that stale selection info floating around after it's relevant
        //  - need to do it before removing the soulBehaviour so that the clause gets evaluated after all the
        //    effects of the soulBehaviour have been resolved
        ContSkillEngine.PushSingleClause(new ClauseClearStoredSelection(soulBehaviour.skillSource));

        //Unsubscribe from all of the potential cancellation triggers
        for (int i = lstPotentialChannelCancelTriggers.Count-1; i >= 0; i--) {
            lstPotentialChannelCancelTriggers[i].UnSubscribe(cbInterruptifInvalid);
        }

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
