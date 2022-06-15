using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewHistoryItemSkill : MonoBehaviour {

    public GameObject goChrSourcePortrait;
    public Text txtSkillName;
    public Text txtTargets;

    public Vector3 v3ChrSourcePosLeft;
    public Vector3 v3ChrSourcePosRight;

    public Chr chrSource;

    //For now, we're just making a list of all Characters affected by the
    //  skill that's been used (which, for a TarSkill, for example, could be the owner
    //  of the skill).  Each somehow affected character will be included visually in the log
    public List<Chr> lstChrAffected;


    public void InitHistoryItemSkill(InputSkillSelection inputSkillSelection) {
        //Set up all of visuals for how we're going to initialize our log entry

        SetChrSourceDisplay(inputSkillSelection.chrActing);

        txtSkillName.text = inputSkillSelection.skillSelected.sDisplayName;

        SetTargetsDisplay(inputSkillSelection);


    }

    public void SetChrSourceDisplay(Chr chrSource) {

        //First, decide if our ChrSource image should be on the left or right side (and flipped horizontally)
        RectTransform recttransform = goChrSourcePortrait.GetComponent<RectTransform>();

        if(chrSource.plyrOwner.id == 0) {
            recttransform.anchoredPosition = v3ChrSourcePosLeft;
        } else {
            recttransform.anchoredPosition = v3ChrSourcePosRight;
            recttransform.localScale = new Vector3(-1f, 1f, 1f);
        }

        //Maybe we can do some highlighting or something if it would help

        //Then set the picture to be of the acting character
        string sSprPath = "Images/Chrs/" + chrSource.sName + "/img" + chrSource.sName + "Neutral";

        LibView.AssignSpritePathToObject(sSprPath, goChrSourcePortrait);
    }

    public void SetTargetsDisplay(InputSkillSelection inputSkillSelection) {
        //First, we'll look through the inputSkillSelection to see what targets were affected

        string sTotalTargetsDescription = "";

        //We'll skip over the first target since that will just be the mana cost
        for(int i = 1; i < inputSkillSelection.lstSelections.Count; i++) {
            //Get our ith target to figure out what the ith selection should be represented as, visually
            string sTargetDescription = inputSkillSelection.skillSelected.lstTargets[i].GetHistoryDescription(inputSkillSelection.lstSelections[i]);

            if(sTargetDescription != "") {
                //Add a newline if we're not the first line
                if(sTotalTargetsDescription != "") sTotalTargetsDescription += "\n";

                sTotalTargetsDescription += sTargetDescription;
            }
        }

        //At this point, we've got a 'list' of all of our target descriptions - now we can fill in our textfield displaying these targets
        txtTargets.text = sTotalTargetsDescription;

    }
}
