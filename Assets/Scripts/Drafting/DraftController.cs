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

    public bool bWaitingOnLocalDraftInput;
    public bool bWaitingOnForeignDraftInput;

    public int indexCurDraftStep;
    public List<DraftAction> lstDraftOrder;

    public bool[] arbChrsAvailableToDraft;

    public DraftableChrCollection draftcollection;

    public const int NDRAFTEDCHRSPERPLAYER = 7;

    public CharType.CHARTYPE[][] arDraftedChrs = new CharType.CHARTYPE[Player.MAXPLAYERS][];
    public int[] arNumChrsDrafted = new int[Player.MAXPLAYERS];

    public DraftedChrDisplay[] arDraftedChrDisplay = new DraftedChrDisplay[Player.MAXPLAYERS];

    public Subject subBeginChooseLocally = new Subject();
    public Subject subEndChooseLocally = new Subject();

    public Subject subBeginChooseForeign = new Subject();
    public Subject subEndChooseForeign = new Subject();

    public Button btnStartDraft;


    public void ProcessDraftInputBuffer() {
        //First, check if we've progressed through the whole draft
        if (IsDraftPhaseOver()) {
            FinishDraft();
            return;
        }

        //Next, check if we have the input that we're waiting for
        if(NetworkDraftReceiver.Get().IsCurSelectionReady() == false) {
            Debug.LogFormat("Still waiting on {0}th draft input", NetworkDraftReceiver.Get().indexCurDraftInput);

            //Check if we're responsible for filling that draft input
            if (NetworkMatchSetup.IsLocallyOwned(GetActivePlayerForNextDraftPhaseStep())) {
                BeginSelectingLocally();
            } else {
                //If we're not responsible for filling that draft input, then locally display that we're waiting for foreign input
                BeginSelectingForeign();
            }
            return;
        }

        //If we do have the input we were waiting on, then let's process it
        DraftAction draftaction = GetNextDraftPhaseStep();
        CharType.CHARTYPE chartypeInput = NetworkDraftReceiver.Get().GetCurSelection();

        if(draftaction.draftactionType == DraftAction.DRAFTACTIONTYPE.DRAFT) {
            //Draft the character
            DraftChr(draftaction.iPlayer, chartypeInput);
        }else {
            //Ban the character
            BanChr(draftaction.iPlayer, chartypeInput);
        }

        //Do any cleanup of local display now that the input we were waiting (or maybe not waiting) on is done
        EndWaitingOnDraftInput();


        //Now that the draft/ban is complete and processed, increment our 'current' indices
        NetworkDraftReceiver.Get().FinishCurSelection();
        FinishDraftPhaseStep();

        //Finally, try to continue to process further buffer events (if they exist)
        ProcessDraftInputBuffer();
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

    public void DraftChr(int iPlayer, CharType.CHARTYPE chrDrafted) {
        //Ensure the character actually exists
        Debug.Assert(chrDrafted < CharType.CHARTYPE.LENGTH);

        //Ensure this character hasn't already been drafted/banned
        Debug.Assert(arbChrsAvailableToDraft[(int)chrDrafted] == true);

        Debug.Log("Drafting " + chrDrafted + " for " + iPlayer);

        //Ensure the draft selection has been registered in the roomoptions (maybe someone else beat us to it, but that's fine)
        NetworkMatchSetup.SetCharacterSelection(iPlayer, arNumChrsDrafted[iPlayer], chrDrafted);

        //Then ensure that everything locally is tracked and displayed properly
        arDraftedChrs[iPlayer][arNumChrsDrafted[iPlayer]] = chrDrafted;
        arNumChrsDrafted[iPlayer]++;

        arbChrsAvailableToDraft[(int)chrDrafted] = false;

        draftcollection.SetChrAsDrafted((int)chrDrafted);
        arDraftedChrDisplay[iPlayer].UpdateDraftedChrDisplays(arDraftedChrs[iPlayer]);

    }

    public void BanChr(int iPlayer, CharType.CHARTYPE chrBanned) {
        //Ensure the character actually exists
        Debug.Assert(chrBanned < CharType.CHARTYPE.LENGTH);

        //Ensure this character hasn't already been drafted/banned
        Debug.Assert(arbChrsAvailableToDraft[(int)chrBanned] == true);

        Debug.Log("Banning " + chrBanned);

        //We shouldn't need to save any bans in the room options since it's only relevent in this scene
        // We still need to update our local information and display this ban properly

        arbChrsAvailableToDraft[(int)chrBanned] = false;

        draftcollection.SetChrAsBanned((int)chrBanned);

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
        //Increment the current draft step we're on
        indexCurDraftStep++;
    }

    public void BeginSelectingLocally() {
        bWaitingOnLocalDraftInput = true;

        //Let anyone (UI effects probably) know that we've begun locally selecting
        subBeginChooseLocally.NotifyObs();
    }

    public void BeginSelectingForeign() {
        bWaitingOnForeignDraftInput = true;

        //Let anyone (UI effects probably) know that we're waiting for another player to make a selection
        subBeginChooseForeign.NotifyObs();
    }

    //Figure out what type of draft action we were waiting on (foreign/local) and react appropriately to completing it
    public void EndWaitingOnDraftInput() {

        //Let anyone (UI effects probably) know that the current player has finished their selection
        if (bWaitingOnLocalDraftInput) {
            subEndChooseLocally.NotifyObs();
            bWaitingOnLocalDraftInput = false;
        } else if (bWaitingOnForeignDraftInput) {
            subEndChooseForeign.NotifyObs();
            bWaitingOnForeignDraftInput = false;
        }
    }
    

    public void OnDraftableChrClicked(CharType.CHARTYPE chrClicked) {

        //Check if we've been told by the master to choose a character to draft/ban
        if(bWaitingOnLocalDraftInput == false) {
            Debug.Log("We haven't been asked to draft/ban a character currently");
            return;
        }

        //Check if we're still in the draft phase
        if(IsDraftPhaseOver() == true) {
            Debug.LogError("Can't draft/ban if the draft phase is over");
            return;
        }

        //Check if it's even our turn to draft
        if(NetworkMatchSetup.IsLocallyOwned(GetActivePlayerForNextDraftPhaseStep()) == false) {
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
        if(GetNextDraftPhaseStep().draftactionType == DraftAction.DRAFTACTIONTYPE.BAN){

            Debug.Log("Sending ban of " + chrClicked);
            NetworkDraftSender.Get().SendBan(chrClicked);

        } else if (GetNextDraftPhaseStep().draftactionType == DraftAction.DRAFTACTIONTYPE.DRAFT) {

            Debug.Log("Sending draft of " + chrClicked);
            NetworkDraftSender.Get().SendDraft(chrClicked);

        }
    }

    public void StartDraft() {

        //Deactivate the 'start draft' button
        btnStartDraft.gameObject.SetActive(false);

        //Start processing the draft input buffer
        ProcessDraftInputBuffer();
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
            arDraftedChrs[i] = new CharType.CHARTYPE[NDRAFTEDCHRSPERPLAYER];
            for(int j=0; j<arDraftedChrs[i].Length; j++) {
                arDraftedChrs[i][j] = CharType.CHARTYPE.LENGTH; //Initially set the chosen character to a flag meaning no selected character yet
            }
        }

        InitChrsAvailableToDraft();

        InitDraftOrder();

    }

    // Update is called once per frame
    void Update() {

    }
}
