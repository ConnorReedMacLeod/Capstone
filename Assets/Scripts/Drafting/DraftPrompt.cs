using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraftPrompt : MonoBehaviour{

    public Text txtDraftPrompt;

    public void cbStartLocalSelection(Object tar, params object[] args) {
        DraftController.DraftAction curDraftStep = DraftController.Get().GetNextDraftPhaseStep();

        if (curDraftStep.draftactionType == DraftController.DraftAction.DRAFTACTIONTYPE.BAN) {
            txtDraftPrompt.text = string.Format("Ban a character for Player {0}", curDraftStep.iPlayer);
        }else if (curDraftStep.draftactionType == DraftController.DraftAction.DRAFTACTIONTYPE.DRAFT) {
            txtDraftPrompt.text = string.Format("Draft a character for Player {0}", curDraftStep.iPlayer);
        }
    }

    public void cbStartForeignSelection(Object tar, params object[] args) {
        DraftController.DraftAction curDraftStep = DraftController.Get().GetNextDraftPhaseStep();

        if (curDraftStep.draftactionType == DraftController.DraftAction.DRAFTACTIONTYPE.BAN) {
            txtDraftPrompt.text = string.Format("Waiting for Player {0} to ban", curDraftStep.iPlayer);
        } else if (curDraftStep.draftactionType == DraftController.DraftAction.DRAFTACTIONTYPE.DRAFT) {
            txtDraftPrompt.text = string.Format("Waiting for Player {0} to draft", curDraftStep.iPlayer);
        }
    }

    public void cbEndSelection(Object tar, params object[] args) {
        txtDraftPrompt.text = "";
    }


    public void Start() {
        DraftController.Get().subBeginChooseForeign.Subscribe(cbStartForeignSelection);
        DraftController.Get().subBeginChooseLocally.Subscribe(cbStartLocalSelection);

        DraftController.Get().subEndChooseForeign.Subscribe(cbEndSelection);
        DraftController.Get().subEndChooseLocally.Subscribe(cbEndSelection);
    }
}
