using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraftController : Singleton<DraftController> {

    public class DraftAction {
        public ContTurns.STATETURN stateTurnNextStep;
        public int iPlayer;

        public DraftAction(ContTurns.STATETURN _stateTurnNextStep, int _iPlayer) {
            stateTurnNextStep = _stateTurnNextStep;
            iPlayer = _iPlayer;
        }

    }

    public bool bActivelyLocallySelecting;

    public Queue<DraftAction> queueDraftOrder;

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

        arDraftedChrs[iPlayer][arNumChrsDrafted[iPlayer]] = chrDrafted;
        arNumChrsDrafted[iPlayer]++;

        arbChrsAvailableToDraft[(int)chrDrafted] = false;

        draftcollection.SetChrAsDrafted((int)chrDrafted);
        arDraftedChrDisplay[iPlayer].UpdateDraftedChrDisplays(arDraftedChrs[iPlayer]);

    }

    public void BanChr(CharType.CHARTYPE chrBanned) {
        //Ensure the character actually exists
        Debug.Assert(chrBanned < CharType.CHARTYPE.LENGTH);

        //Ensure this character hasn't already been drafted/banned
        Debug.Assert(arbChrsAvailableToDraft[(int)chrBanned] == true);

        Debug.Log("Banning " + chrBanned);

        arbChrsAvailableToDraft[(int)chrBanned] = false;

        draftcollection.SetChrAsBanned((int)chrBanned);

    }

    public void InitDraftOrder() {
        queueDraftOrder = new Queue<DraftAction>();
        //p1 ban
        queueDraftOrder.Enqueue(new DraftAction(ContTurns.STATETURN.CHOOSEBAN, 0));
        //p2 ban
        queueDraftOrder.Enqueue(new DraftAction(ContTurns.STATETURN.CHOOSEBAN, 1));
        //p1 pick
        queueDraftOrder.Enqueue(new DraftAction(ContTurns.STATETURN.CHOOSEDRAFT, 0));
        //p2 pick pick
        queueDraftOrder.Enqueue(new DraftAction(ContTurns.STATETURN.CHOOSEDRAFT, 1));
        queueDraftOrder.Enqueue(new DraftAction(ContTurns.STATETURN.CHOOSEDRAFT, 1));
        //p1 pick pick
        queueDraftOrder.Enqueue(new DraftAction(ContTurns.STATETURN.CHOOSEDRAFT, 0));
        queueDraftOrder.Enqueue(new DraftAction(ContTurns.STATETURN.CHOOSEDRAFT, 0));
        //p2 pick pick
        queueDraftOrder.Enqueue(new DraftAction(ContTurns.STATETURN.CHOOSEDRAFT, 1));
        queueDraftOrder.Enqueue(new DraftAction(ContTurns.STATETURN.CHOOSEDRAFT, 1));
        //p1 pick pick
        queueDraftOrder.Enqueue(new DraftAction(ContTurns.STATETURN.CHOOSEDRAFT, 0));
        queueDraftOrder.Enqueue(new DraftAction(ContTurns.STATETURN.CHOOSEDRAFT, 0));
        //p2 pick
        queueDraftOrder.Enqueue(new DraftAction(ContTurns.STATETURN.CHOOSEDRAFT, 1));

    }

    public bool IsDraftPhaseOver() {
        return queueDraftOrder.Count == 0;
    }

    public DraftAction GetNextDraftPhaseStep() {
        Debug.Assert(IsDraftPhaseOver() == false, "Asked for a next draft phase when there are no more steps left");

        return queueDraftOrder.Peek();

    }

    public int GetActivePlayerForNextDraftPhaseStep() {
        return GetNextDraftPhaseStep().iPlayer;
    }

    public void FinishDraftPhaseStep() {
        Debug.Assert(IsDraftPhaseOver() == false, "Attempted to finish a draft phase step when there are already none left");

        queueDraftOrder.Dequeue();
    }

    public void BeginSelectingLocally() {

        //First, check if we've actually received 

        bActivelyLocallySelecting = true;

        //Let anyone (UI effects probably) know that we've begun locally selecting
        subBeginChooseLocally.NotifyObs();
    }

    public void BeginSelectingForegin() {

        //Let anyone (UI effects probably) know that we've begun locally selecting
        subBeginChooseForeign.NotifyObs();
    }

    public void EndSelecting() {

        //Let anyone (UI effects probably) know that we've ended locally selecting
        if (bActivelyLocallySelecting) {
            subEndChooseLocally.NotifyObs();
        } else {
            subEndChooseForeign.NotifyObs();
        }

        bActivelyLocallySelecting = false;
    }
    

    public void OnDraftableChrClicked(CharType.CHARTYPE chrClicked) {

        //Check if we've been told by the master to choose a character to draft/ban
        if(bActivelyLocallySelecting == false) {
            Debug.Log("We haven't been asked to draft/ban a character currently");
            return;
        }

        //Check if we're still in the draft phase
        if(IsDraftPhaseOver() == true) {
            Debug.LogError("Can't draft/ban if the draft phase is over");
            return;
        }

        //Check if it's even our turn to draft
        if(ClientNetworkController.Get().IsPlayerLocallyControlled(GetActivePlayerForNextDraftPhaseStep()) == false) {
            Debug.LogError("Can't draft/ban since it's not your turn");
            return;
        }

        //Check if this character is available to draft/ban
        if(IsCharAvailable(chrClicked) == false) {
            Debug.LogError("Can't draft/ban an unavailable character");
            return;
        }

        Debug.Log("NextStep of draft is currently " + GetNextDraftPhaseStep().stateTurnNextStep);

        //At this point, it's valid to pick/ban the character so send along the appropriate signal to the Master
        if(GetNextDraftPhaseStep().stateTurnNextStep == ContTurns.STATETURN.CHOOSEBAN) {
            Debug.Log("Sending ban of " + chrClicked);
            ClientNetworkController.Get().SendTurnPhaseFinished(ContTurns.STATETURN.CHOOSEBAN, new int[1] { (int)chrClicked });
        } else if(GetNextDraftPhaseStep().stateTurnNextStep == ContTurns.STATETURN.CHOOSEDRAFT) {
            Debug.Log("Sending draft of " + chrClicked);
            ClientNetworkController.Get().SendTurnPhaseFinished(ContTurns.STATETURN.CHOOSEDRAFT, new int[1] { (int)chrClicked });
        }
    }

    public void StartDraft() {
        Debug.Log("Sending start draft signal to the master");
        ClientNetworkController.Get().SendTurnPhaseFinished(ContTurns.STATETURN.STARTDRAFT, null);

        btnStartDraft.gameObject.SetActive(false);
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
