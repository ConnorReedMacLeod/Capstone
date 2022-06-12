using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewHistoryItemSkill : MonoBehaviour {

    public GameObject pfChrPortrait;

    public Chr chrSource;

    //For now, we're just making a list of all Characters affected by the
    //  skill that's been used (which, for a TarSkill, for example, could be the owner
    //  of the skill).  Each somehow affected character will be included visually in the log
    public List<Chr> lstChrAffected;


    public void InitHistoryItemSkill(InputSkillSelection inputSkillSelection) {
        //Set up all of visuals for how we're going to initialize our log entry


        //First, we'll look through the inputSkillSelection to see which characters were affected
        chrSource = inputSkillSelection.chrActing;

        lstChrAffected = new List<Chr>();

        for(int i = 0; i < inputSkillSelection.lstSelections.Count; i++) {

        }
    }

}
