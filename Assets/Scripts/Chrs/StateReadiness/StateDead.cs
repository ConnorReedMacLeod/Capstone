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

    public override bool CanSelectSkill(Skill skill) {
        //Can't select a skill while dead
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

        //Debug.Log("nLive Characters is now " + ContTurns.Get().nLiveCharacters + " since " + chrOwner.sName + " just died");

        //After fixing priority ordering (pushing this character to the back)

        //TODO do a check for if the game should be over
        /*
          if(chrNextToBlock == null) {
            Debug.Log("Game should end");
            Player.subAllPlayerLost.NotifyObs(chrOwner.plyrOwner);
            ContTime.Get().Pause();
        } else {
            chrOwner.plyrOwner.SetDefaultBlocker();
        }

        */

        Debug.Log("Remember to dispell soul effects on dying characters");

        chrOwner.subDeath.NotifyObs(chrOwner);
        Chr.subAllDeath.NotifyObs(chrOwner);
    }
}
