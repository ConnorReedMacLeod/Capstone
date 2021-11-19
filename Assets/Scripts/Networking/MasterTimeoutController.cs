using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterTimeoutController : MonoBehaviour {

    public MasterNetworkController master;

    public const float fTimeoutToStartMatch = 30f;
    public const float fTimeoutStandard = 15f;


    public float fTimeoutTimer;
    public ContTurns.STATETURN stateTurnWaitingOn;


    public void StartTimeoutTimer(ContTurns.STATETURN stateTurn) {

        if(fTimeoutTimer != 0.0f) {
            //If the timer is already going, then we don't need to start it again
            return;
        }

        //Save the current turnstate we're processing
        stateTurnWaitingOn = stateTurn;

        if (stateTurn == ContTurns.STATETURN.CHOOSESKILL) {
            //TODO - only enforce the full time if we're waiting on the active player
            //just piggy-back off the local player's selection
            // TODO - sync this variable up among all players
            fTimeoutTimer = ContSkillSelection.Get().fMaxSelectionTime;
        } else if (stateTurn == ContTurns.STATETURN.STARTDRAFT) {
            fTimeoutTimer = fTimeoutToStartMatch;
        } else {
            fTimeoutTimer = fTimeoutStandard;
        }

    }

    public void TimeoutReached() {

        Debug.Log("Timeout reached");

        //If the time limit has been reached, force all players to move onto the next
        //  phase of the turn (whether ready or not) - TODO - does this make sense?
        master.ForceAllClientsEndPhase(stateTurnWaitingOn);

        EndTimeoutTimer();
    }

    public void EndTimeoutTimer() {
        fTimeoutTimer = 0.0f;
    }

    private void OnEnable() {

        master = GetComponent<MasterNetworkController>();

        Debug.Assert(master != null, "ERROR - no MasterNetworkController component found with MasterTimeoutController");

    }

    // Update is called once per frame
    void Update() {

        if(fTimeoutTimer != 0.0f) {
            fTimeoutTimer -= Time.deltaTime;

            if(fTimeoutTimer < 0.0f) {
                TimeoutReached();
            }
        }

    }
}
