using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateDead : StateReadiness {

    public StateDead(Chr _chrOwner) : base(_chrOwner) {

    }

    public override TYPE Type() {
        return TYPE.DEAD;
    }

    public override void Ready() {
        //Can't ready when dead
    }

    public override bool CanSelectAction(Action act) {
        //Can't select an action while dead
        return false;
    }

    public override int GetPriority() {
        //return an impossibly high priority value so we can't be selected
        return 999999;
    }

    public override void Recharge() {
        //Nothing should be recharged when dead
        
    }

    public override void OnEnter() {
        //Do any death-related effects

        chrOwner.bDead = true;

        //Fix our position in the priority queue (we should now be after all living characters)
        ContTurns.Get().FixSortedPriority(chrOwner);

        //Once we're done swapping, this dead character should be at the end of the living section of the list
        //so reduce the size of the living section of the list by one so they're no longer included
        ContTurns.Get().nLiveCharacters--;

        //Debug.Log("nLive Characters is now " + ContTurns.Get().nLiveCharacters + " since " + chrOwner.sName + " just died");

        //After fixing priority ordering (pushing this character to the back)
        //If the character is the blocker, then change the blocker to the next character to act
        Chr chrNextToBlock = ContTurns.Get().GetNextToActOwnedBy(chrOwner.plyrOwner);

        if (chrNextToBlock == null) {
            Debug.Log("Game should end");
            Player.subAllPlayerLost.NotifyObs(chrOwner.plyrOwner);
            ContTime.Get().Pause();
        } else {
            chrOwner.plyrOwner.SetDefaultBlocker();
        }

        chrOwner.subDeath.NotifyObs(chrOwner);
        Chr.subAllDeath.NotifyObs(chrOwner);
    }
}
