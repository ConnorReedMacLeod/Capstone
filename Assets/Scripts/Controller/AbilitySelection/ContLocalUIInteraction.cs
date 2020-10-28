using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class handles the UI interaction flow for clicking on characters/skills/selections
//
// It maintains the state of the interaction (i.e. if we're not currently selecting any
//  character, if we're selecting a character and showing their skills, if we're selecting
//  the target for a particular skill (and which type of entity we're trying to select dependent
//  on the ability.
//
// Will often consule the LocalInputType to see what interactions are possible to proceed
//  with given the control-level we have for the local player (i.e., if the local
//  player is human then we have permission to move ahead with selecting abilities.  If
//  the local player is an AI, then we should only be able to click characters and hover over
//  skills to see information on them, but not to take any action with them.
public class ContLocalUIInteraction : Singleton<ContLocalUIInteraction> {

    public StateTarget curState;

    public Chr chrSelected;

    public Action actSelected;

    //Note - for now, we're assuming we'll only ever target one thing (or, more broadly,
    //  require one click for finalizing selection of an ability's targets).  If this ever changes,
    //  we can make a list of needed selection types and record the chosen selections here


    public static Subject subAllStartTargetting = new Subject(Subject.SubType.ALL);
    public static Subject subAllFinishTargetting = new Subject(Subject.SubType.ALL);

    // Start a new round of targetting
    public void ResetTar() {
        //Clear any previous targetting information we had
        actSelected = null;

    }

    // Ends targetting
    public void CancelTar() {

        //Potentially don't fully reset to the idle state if we're just selecting
        // a character, but not selecting targets for an ability of theirs

        SetState(new StateTargetIdle());

        //Let everything know that targetting has ended
        subAllFinishTargetting.NotifyObs(this);
    }

    public void FinishTargetting() {

        //Only allow manual selections when the local player is human
        Debug.Assert(Match.Get().GetLocalPlayer().curInputType == Player.InputType.HUMAN);

        ContAbilitySelection.Get().SubmitAbility(chrSelected.plyrOwner.inputController);

        // Can now go back idle and wait for the next targetting
        SetState(new StateTargetIdle());

        //Let everything know that targetting has ended
        subAllFinishTargetting.NotifyObs(this);
    }

    // Create the necessary state for selecting the needed type
    public void SetTargetArgState() {

        if(iTargetIndex < 0) {
            //Then we've cancelled the targetting action so go back to... idle?
            CancelTar();

        } else if(iTargetIndex == 0) {
            //Then create the targetting array with the correct size
            arTargetIndices = new int[chrSelected.arActions[nSelectedAbility].nArgs];

            //Then we should let things know that a new targetting has begun
            subAllStartTargetting.NotifyObs(chrSelected, nSelectedAbility);
        }

        if(iTargetIndex == chrSelected.arActions[nSelectedAbility].nArgs) {
            //Then we've filled of the targetting arguments

            //Debug.Log ("Targetting finished");

            //Submit our targetting selections to the InputAbilitySelection controller
            FinishTargetting();

        } else {



            // Get the type of the target arg that we need to handle
            Clause.TargetType tarType = actSelected.GetTargetType();

            switch(tarType) {
            case Clause.TargetType.ACTION:
                //TODONOW
                break;

            case Clause.TargetType.PLAYER:
                //TODONOW
                break;

            case Clause.TargetType.CHR:
                newState = new StateTargetChr();
                break;

            case Clause.TargetType.SPECIAL:
                //TODONOW
                break;

            default:

                Debug.LogError(sArgType + " is not a recognized ArgType!");
                return;
            }

            // Transition to the appropriate state
            SetState(newState);

        }
    }

    public void SetState(StateTarget newState) {

        if(curState != null) {
            //Debug.Log("Leaving State " + curState.ToString());
            curState.OnLeave();
        }

        curState = newState;

        if(curState != null) {
            curState.OnEnter();
            //Debug.Log("Entering State " + curState.ToString());
        }
    }

    public override void Init() {

        SetState(new StateTargetIdle());

    }


    public ContLocalUIInteraction() {

        iTargetIndex = 0;
    }
}
