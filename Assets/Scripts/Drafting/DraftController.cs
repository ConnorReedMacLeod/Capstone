﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftController : SingletonPersistent<DraftController> {

    public class DraftAction {
        public ContTurns.STATETURN stateTurnNextStep;
        public int iPlayer;

        public DraftAction(ContTurns.STATETURN _stateTurnNextStep, int _iPlayer) {
            stateTurnNextStep = _stateTurnNextStep;
            iPlayer = _iPlayer;
        }

    }


    public Queue<DraftAction> queueDraftOrder;

    public bool[] arbChrsAvailableToDraft;

    public DraftableChrCollection draftcollection;

    public const int NDRAFTEDCHRSPERPLAYER = 7;

    public Chr.CHARTYPE[][] arDraftedChrs = new Chr.CHARTYPE[2][];
    public int[] arNumChrsDrafted = new int[2];

    public DraftedChrDisplay[] arDraftedChrDisplay = new DraftedChrDisplay[2];

    public void InitAllDraftingPortraits() {

        arbChrsAvailableToDraft = new bool[(int)Chr.CHARTYPE.LENGTH];

        for(int i = 0; i < arbChrsAvailableToDraft.Length; i++) {
            arbChrsAvailableToDraft[i] = true;
        }

        draftcollection.UpdateDraftableCharacterPortraits(arbChrsAvailableToDraft);

    }

    public bool IsCharAvailable(Chr.CHARTYPE chr) {
        //If the selection is an invalid one (used for no selection)
        if(chr == Chr.CHARTYPE.LENGTH) return false;

        return arbChrsAvailableToDraft[(int)chr];
    }

    public void DraftChr(int iPlayer, Chr.CHARTYPE chrDrafted) {
        //Ensure the character actually exists
        Debug.Assert(chrDrafted == Chr.CHARTYPE.LENGTH);

        //Ensure this character hasn't already been drafted/banned
        Debug.Assert(arbChrsAvailableToDraft[(int)chrDrafted] == true);

        arDraftedChrs[iPlayer][arNumChrsDrafted[iPlayer]] = chrDrafted;

        arbChrsAvailableToDraft[(int)chrDrafted] = false;

        draftcollection.SetChrAsDrafted((int)chrDrafted);
        arDraftedChrDisplay[iPlayer].UpdateDraftedChrDisplay(arDraftedChrs[iPlayer]);

    }

    public void BanChr(Chr.CHARTYPE chrBanned) {
        //Ensure the character actually exists
        Debug.Assert(chrBanned == Chr.CHARTYPE.LENGTH);

        //Ensure this character hasn't already been drafted/banned
        Debug.Assert(arbChrsAvailableToDraft[(int)chrBanned] == true);

        arbChrsAvailableToDraft[(int)chrBanned] = false;

        draftcollection.SetChrAsBanned((int)chrBanned);

    }

    public void InitDraftOrder() {
        queueDraftOrder = new Queue<DraftAction>();
        //p1 ban
        queueDraftOrder.Enqueue(new DraftAction(ContTurns.STATETURN.BAN, 0));
        //p2 ban
        queueDraftOrder.Enqueue(new DraftAction(ContTurns.STATETURN.BAN, 1));
        //p1 pick
        queueDraftOrder.Enqueue(new DraftAction(ContTurns.STATETURN.DRAFT, 0));
        //p2 pick pick
        queueDraftOrder.Enqueue(new DraftAction(ContTurns.STATETURN.DRAFT, 1));
        queueDraftOrder.Enqueue(new DraftAction(ContTurns.STATETURN.DRAFT, 1));
        //p1 pick pick
        queueDraftOrder.Enqueue(new DraftAction(ContTurns.STATETURN.DRAFT, 0));
        queueDraftOrder.Enqueue(new DraftAction(ContTurns.STATETURN.DRAFT, 0));
        //p2 pick pick
        queueDraftOrder.Enqueue(new DraftAction(ContTurns.STATETURN.DRAFT, 1));
        queueDraftOrder.Enqueue(new DraftAction(ContTurns.STATETURN.DRAFT, 1));
        //p1 pick pick
        queueDraftOrder.Enqueue(new DraftAction(ContTurns.STATETURN.DRAFT, 0));
        queueDraftOrder.Enqueue(new DraftAction(ContTurns.STATETURN.DRAFT, 0));
        //p2 pick
        queueDraftOrder.Enqueue(new DraftAction(ContTurns.STATETURN.DRAFT, 1));

    }

    public bool IsDraftPhaseOver() {
        return queueDraftOrder.Count == 0;
    }

    public DraftAction GetNextDraftPhaseStep() {
        Debug.Assert(queueDraftOrder.Count != 0, "Asked for a next draft phase when there are no more steps left");

        return queueDraftOrder.Peek();

    }

    public int GetActivePlayerForNextDraftPhaseStep() {
        return GetNextDraftPhaseStep().iPlayer;
    }

    public void FinishDraftPhaseStep() {
        Debug.Assert(queueDraftOrder.Count != 0, "Attempted to finish a draft phase step when there are already none left");

        queueDraftOrder.Dequeue();
    }


    public void OnDraftableChrClicked(Chr.CHARTYPE chrClicked) {

        //Check if we're still in the draft phase
        if(IsDraftPhaseOver() == true) {
            Debug.Log("Can't draft/ban if the draft phase is over");
            return;
        }

        //Check if it's even our turn to draft
        if(ClientNetworkController.Get().IsPlayerLocallyControlled(GetActivePlayerForNextDraftPhaseStep()) == false) {
            Debug.Log("Can't draft/ban since it's not your turn");
            return;
        }

        //Check if this character is available to draft/ban
        if(IsCharAvailable(chrClicked) == false) {
            Debug.Log("Can't draft/ban an unavailable character");
            return;
        }

        //At this point, it's valid to pick/ban the character so send along the appropriate signal to the Master
        if(GetNextDraftPhaseStep().stateTurnNextStep == ContTurns.STATETURN.BAN) {
            NetworkConnectionManager.SendEventToMaster(MasterNetworkController.evtMBanCharacterSelected, new object[1] { chrClicked });
        } else if(GetNextDraftPhaseStep().stateTurnNextStep == ContTurns.STATETURN.DRAFT) {
            NetworkConnectionManager.SendEventToMaster(MasterNetworkController.evtMDraftCharacterSelected, new object[1] { chrClicked });
        }
    }


    public override void Init() {

        for(int i = 0; i < arDraftedChrs.Length; i++) {
            arDraftedChrs[i] = new Chr.CHARTYPE[NDRAFTEDCHRSPERPLAYER];
        }

        InitAllDraftingPortraits();

        InitDraftOrder();

    }

    // Update is called once per frame
    void Update() {

    }
}
