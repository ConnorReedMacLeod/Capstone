using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewHistoryPanel : Singleton<ViewHistoryPanel> {

    public ScrollRect scrollRect;

    public GameObject pfHistoryItemSkill;

    public float fCurHistoryItemY;


    public void AddHistoryItemSkill(InputSkillSelection inputSkillSelection) {

        //Debug.LogFormat("Scrollvalue was {0}", scrollRect.verticalNormalizedPosition);

        //Ensure that the scrollbar progress is set to the bottom (the most recent item)
        // - note that this is done after a small delay since doing it immediately would be
        //   overwritten by the automatic Unity layout group stuff
        if(scrollRect.verticalNormalizedPosition < 0.05f) {
            Invoke("ResetScrollbar", 0.1f);
        }

        //Instantiate a new history item at the current position
        GameObject goNewHistoryItem = GameObject.Instantiate(pfHistoryItemSkill, scrollRect.content.transform);

        //Set up the newly added History Item
        goNewHistoryItem.GetComponent<ViewHistoryItemSkill>().InitHistoryItemSkill(inputSkillSelection);


    }


    public override void Init() {
        fCurHistoryItemY = 0f;
    }

    public void ResetScrollbar() {

        scrollRect.verticalNormalizedPosition = 0f;
    }

}
