using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraftController : Singleton<DraftController> {

    public class DraftAction {
        public enum DRAFTACTIONTYPE { DRAFT, BAN };

        public DRAFTACTIONTYPE draftactionType;
        public int iPlayer;

        public DraftAction(DRAFTACTIONTYPE _draftactionType, int _iPlayer) {
            draftactionType = _draftactionType;
            iPlayer = _iPlayer;
        }

    }

    public DraftAction draftactionWaitingOn;

    public int indexCurDraftStep;
    public List<DraftAction> lstDraftOrder;

    public bool[] arbChrsAvailableToDraft;

    public DraftableChrCollection draftcollection;

    public CharType.CHARTYPE[][] arDraftedChrs = new CharType.CHARTYPE[Match.NPLAYERS][];
    public int[] arNumChrsDrafted = new int[Match.NPLAYERS];

    public DraftedChrDisplay[] arDraftedChrDisplay = new DraftedChrDisplay[Match.NPLAYERS];

    public Subject subBeginChooseLocally = new Subject();
    public Subject subEndChooseLocally = new Subject();

    public Subject subBeginChooseForeign = new Subject();
    public Subject subEndChooseForeign = new Subject();

    public Button btnStartDraft;

    //The main loop that will spin waiting for outside networked input and process
    //  it, once our local simulation is ready for a new input
    public IEnumerator CRDraftLoop() {

        //Do any animation processing that needs to be done before the draft input actually starts
        yield return StartCoroutine(CRPrepDraft());

        Debug.Log("Done prepping draft");


        //Keep processing turn events while the draft isn't finished
        while(!IsDraftPhaseOver()) {

            //Check if we have input waiting for us in the network buffer
            while(NetworkDraftReceiver.Get().IsCurSelectionReady() == false) {
                //Keep spinning until we get the input we're waiting on

                WaitForDraftInput();
                yield return null;
            }
            //If we were waiting on input, then clean up the waiting process
            if(draftactionWaitingOn != null) EndWaitingOnDraftInput();

            //At this point, we have an input in the buffer that we are able to process
            DraftAction draftaction = GetNextDraftPhaseStep();
            CharType.CHARTYPE chartypeInput = NetworkDraftReceiver.Get().GetCurSelection();

            Debug.Log("Got a draft action of type " + draftaction.draftactionType + " for chr " + chartypeInput);

            //Start a coroutine to process whichever event we need to execute
            if(draftaction.draftactionType == DraftAction.DRAFTACTIONTYPE.DRAFT) {
                //Draft the character
                yield return StartCoroutine(CRDraftChr(draftaction.iPlayer, chartypeInput));
            } else {
                //Ban the character
                yield return StartCoroutine(CRBanChr(draftaction.iPlayer, chartypeInput));
            }

            //Now that the draft/ban is complete and processed, increment our 'current' indices
            NetworkDraftReceiver.Get().FinishCurSelection();
            FinishDraftPhaseStep();
        }

        //Do any animation process that needs to be done before we leave the draft scene
        yield return StartCoroutine(CRCleanUpDraft());

        //Wrap up the draft phase
        FinishDraft();
        yield break;
    }

    //Do any initial intro animations and loading for before we start processing draft input
    public IEnumerator CRPrepDraft() {

        Debug.Log("Starting up the draft");

        yield return new WaitForSeconds(1.0f);

    }

    //Do any cleanup animations for the end of the draft
    public IEnumerator CRCleanUpDraft() {

        Debug.Log("Cleaning up the draft");

        yield return new WaitForSeconds(1.0f);

    }

    public void InitChrsAvailableToDraft() {

        //Get all possible characters that can be drafted and initialize a draftable array for each char type
        arbChrsAvailableToDraft = new bool[(int)CharType.CHARTYPE.LENGTH];

        for(int i = 0; i < arbChrsAvailableToDraft.Length; i++) {
            arbChrsAvailableToDraft[i] = true;
        }


    }

    public bool IsCharAvailable(CharType.CHARTYPE chr) {
        //If the selection is an invalid one (used for no selection)
        if(chr == CharType.CHARTYPE.LENGTH) return false;

        return arbChrsAvailableToDraft[(int)chr];
    }

    public IEnumerator CRDraftChr(int iPlayer, CharType.CHARTYPE chrDrafted) {
        //Ensure the character actually exists
        Debug.Assert(chrDrafted < CharType.CHARTYPE.LENGTH);

        //Ensure this character hasn't already been drafted/banned
        Debug.Assert(arbChrsAvailableToDraft[(int)chrDrafted] == true);

        Debug.Log("Drafting " + chrDrafted + " for " + iPlayer);

        //Ensure the draft selection has been registered in the roomoptions (maybe someone else beat us to it, but that's fine)
        NetworkMatchSetup.SetDraftedCharacter(iPlayer, arNumChrsDrafted[iPlayer], chrDrafted);

        //Then ensure that everything locally is tracked and displayed properly
        arDraftedChrs[iPlayer][arNumChrsDrafted[iPlayer]] = chrDrafted;
        arNumChrsDrafted[iPlayer]++;

        arbChrsAvailableToDraft[(int)chrDrafted] = false;

        draftcollection.SetChrAsDrafted((int)chrDrafted);
        arDraftedChrDisplay[iPlayer].UpdateDraftedChrDisplays(arDraftedChrs[iPlayer]);

        yield return new WaitForSeconds(1.0f);

        Debug.Log("Finished 'waiting' for the draft to finish");
    }

    public IEnumerator CRBanChr(int iPlayer, CharType.CHARTYPE chrBanned) {
        //Ensure the character actually exists
        Debug.Assert(chrBanned < CharType.CHARTYPE.LENGTH);

        //Ensure this character hasn't already been drafted/banned
        Debug.Assert(arbChrsAvailableToDraft[(int)chrBanned] == true);

        Debug.Log("Banning " + chrBanned);

        //We shouldn't need to save any bans in the room options since it's only relevent in this scene
        // We still need to update our local information and display this ban properly

        arbChrsAvailableToDraft[(int)chrBanned] = false;

        draftcollection.SetChrAsBanned((int)chrBanned);

        yield return new WaitForSeconds(1.0f);

        Debug.Log("Finished 'waiting' for the ban to finish");
    }

    public void InitDraftOrder() {
        lstDraftOrder = new List<DraftAction>();
        //p1 ban
        lstDraftOrder.Add(new DraftAction(DraftAction.DRAFTACTIONTYPE.BAN, 0));
        //p2 ban
        lstDraftOrder.Add(new DraftAction(DraftAction.DRAFTACTIONTYPE.BAN, 1));
        //p1 pick
        lstDraftOrder.Add(new DraftAction(DraftAction.DRAFTACTIONTYPE.DRAFT, 0));
        //p2 pick pick
        lstDraftOrder.Add(new DraftAction(DraftAction.DRAFTACTIONTYPE.DRAFT, 1));
        lstDraftOrder.Add(new DraftAction(DraftAction.DRAFTACTIONTYPE.DRAFT, 1));
        //p1 pick pick
        lstDraftOrder.Add(new DraftAction(DraftAction.DRAFTACTIONTYPE.DRAFT, 0));
        lstDraftOrder.Add(new DraftAction(DraftAction.DRAFTACTIONTYPE.DRAFT, 0));
        //p2 pick pick
        lstDraftOrder.Add(new DraftAction(DraftAction.DRAFTACTIONTYPE.DRAFT, 1));
        lstDraftOrder.Add(new DraftAction(DraftAction.DRAFTACTIONTYPE.DRAFT, 1));
        //p1 pick pick
        lstDraftOrder.Add(new DraftAction(DraftAction.DRAFTACTIONTYPE.DRAFT, 0));
        lstDraftOrder.Add(new DraftAction(DraftAction.DRAFTACTIONTYPE.DRAFT, 0));
        //p2 pick pick
        lstDraftOrder.Add(new DraftAction(DraftAction.DRAFTACTIONTYPE.DRAFT, 1));
        lstDraftOrder.Add(new DraftAction(DraftAction.DRAFTACTIONTYPE.DRAFT, 1));
        //p1 pick pick
        lstDraftOrder.Add(new DraftAction(DraftAction.DRAFTACTIONTYPE.DRAFT, 0));
        lstDraftOrder.Add(new DraftAction(DraftAction.DRAFTACTIONTYPE.DRAFT, 0));
        //p2 pick
        lstDraftOrder.Add(new DraftAction(DraftAction.DRAFTACTIONTYPE.DRAFT, 1));

    }

    public bool IsDraftPhaseOver() {
        return indexCurDraftStep == lstDraftOrder.Count;
    }

    public DraftAction GetNextDraftPhaseStep() {
        Debug.Assert(IsDraftPhaseOver() == false, "Asked for a next draft phase when there are no more steps left");

        return lstDraftOrder[indexCurDraftStep];

    }

    public int GetActivePlayerForNextDraftPhaseStep() {
        return GetNextDraftPhaseStep().iPlayer;
    }

    public void FinishDraftPhaseStep() {
        Debug.Log("Finished draft phase step " + indexCurDraftStep);

        //Increment the current draft step we're on
        indexCurDraftStep++;
    }

    public void WaitForDraftInput() {
        //If we're already waiting, then we don't need to do anything further
        if(draftactionWaitingOn != null) return;

        //Raise the flag that we're waiting for input
        draftactionWaitingOn = GetNextDraftPhaseStep();

        //Check what type of input we're waiting on
        if(NetworkMatchSetup.IsLocallyOwned(draftactionWaitingOn.iPlayer)) {
            //Prompt the local player to select input
            subBeginChooseLocally.NotifyObs();
        } else {
            //Let anyone (UI effects probably) know that we're waiting for another player to make a selection
            subBeginChooseForeign.NotifyObs();
        }
        return;
    }

    //Figure out what type of draft action we were waiting on (foreign/local) and react appropriately to completing it
    public void EndWaitingOnDraftInput() {

        //Check what type of input we're waiting on
        if(NetworkMatchSetup.IsLocallyOwned(draftactionWaitingOn.iPlayer)) {
            //Let anyone (UI effects probably) know that the current player has finished their selection
            subEndChooseLocally.NotifyObs();
        } else {
            subEndChooseForeign.NotifyObs();
        }

        //Clear the action we were waiting on
        draftactionWaitingOn = null;
    }


    public void OnDraftableChrClicked(CharType.CHARTYPE chrClicked) {

        //Check if we've been told by the master to choose a character to draft/ban
        if(draftactionWaitingOn == null) {
            Debug.Log("We aren't waiting on any input right now");
            return;
        }

        //Check if it's even our turn to draft
        if(NetworkMatchSetup.IsLocallyOwned(draftactionWaitingOn.iPlayer) == false) {
            Debug.LogError("Can't draft/ban since it's not your turn");
            return;
        }

        //Check if this character is available to draft/ban
        if(IsCharAvailable(chrClicked) == false) {
            Debug.LogError("Can't draft/ban an unavailable character");
            return;
        }

        Debug.Log("Current step of draft is " + GetNextDraftPhaseStep().draftactionType);

        //At this point, it's valid to pick/ban the character so send along the appropriate signal to the Master
        if(GetNextDraftPhaseStep().draftactionType == DraftAction.DRAFTACTIONTYPE.BAN) {

            Debug.Log("Sending ban of " + chrClicked);
            NetworkDraftSender.Get().SendBan(chrClicked);

        } else if(GetNextDraftPhaseStep().draftactionType == DraftAction.DRAFTACTIONTYPE.DRAFT) {

            Debug.Log("Sending draft of " + chrClicked);
            NetworkDraftSender.Get().SendDraft(chrClicked);

        }
    }

    public void StartDraft() {

        Debug.Log("Starting draft");

        //Deactivate the 'start draft' button
        btnStartDraft.gameObject.SetActive(false);

        //Start processing the draft input buffer
        StartCoroutine(CRDraftLoop());
    }

    public void FinishDraft() {
        Debug.Log("Need to implement FinishDraft");
        //TODONOW - figure this out exactly
        // Essentially, we'll wait a few seconds (for both players to catch up and view the last drafted character), then 
        //  if we're the master client, we'll move the cohort to the loadout setup scene
        // Could instead add a button to progress to next scene?
    }

    public override void Init() {

        //Set up an array for each draft pick # for each player
        for(int i = 0; i < arDraftedChrs.Length; i++) {
            arDraftedChrs[i] = new CharType.CHARTYPE[Match.NCHRSPERDRAFT];
            for(int j = 0; j < arDraftedChrs[i].Length; j++) {
                arDraftedChrs[i][j] = CharType.CHARTYPE.LENGTH; //Initially set the chosen character to a flag meaning no selected character yet
            }

            arDraftedChrDisplay[i].UpdateDraftedChrDisplays(arDraftedChrs[i]);
        }

        InitChrsAvailableToDraft();

        InitDraftOrder();

    }

    // Update is called once per frame
    void Update() {

    }
}
