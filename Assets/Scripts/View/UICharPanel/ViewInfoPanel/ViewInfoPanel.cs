using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Note - there are some bugs when you hover over the blocker button then it disappears
//        leaving the description panel up
public class ViewInfoPanel : MonoBehaviour {

    public GameObject pfInfoSkill;
    public ViewInfoSkill viewInfoSkill;

    public GameObject goCurInfoContent;

    public void ShowInfoSkill(Skill _mod) {

        if(viewInfoSkill != null && viewInfoSkill.mod == _mod) {
            //Then we're already showing this - no need to change anything
        } else if(viewInfoSkill != null) {
            //Then we're showing something else - just update the model
            viewInfoSkill.SetModel(_mod);
        } else {
            //Then we need to clear the current panel and set up a new InfoSkillPanel
            ClearPanel();

            goCurInfoContent = Instantiate(pfInfoSkill, transform);
            viewInfoSkill = goCurInfoContent.GetComponent<ViewInfoSkill>();
            if(viewInfoSkill == null) {
                Debug.LogError("ERROR!  InfoSkill prefab doesn't have a viewinfoskill component!");
            }
            viewInfoSkill.Start();
            viewInfoSkill.SetModel(_mod);
        }
    }

    public void ClearPanel() {
        viewInfoSkill = null;
        Destroy(goCurInfoContent);
        goCurInfoContent = null;
    }

    public void Start() {

    }
}
