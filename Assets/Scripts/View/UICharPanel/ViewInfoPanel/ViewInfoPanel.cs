using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Note - there are some bugs when you hover over the blocker button then it disappears
//        leaving the description panel up
public class ViewInfoPanel : MonoBehaviour {

	public GameObject pfInfoAction;
	public ViewInfoAction viewInfoAction;

	public GameObject goCurInfoContent;

	public void ShowInfoAction(Action _mod){

		if (viewInfoAction != null && viewInfoAction.mod == _mod) {
			//Then we're already showing this - no need to change anything
		} else if (viewInfoAction != null) {
			//Then we're showing something else - just update the model
			viewInfoAction.SetModel (_mod);
		} else {
			//Then we need to clear the current panel and set up a new InfoActionPanel
			ClearPanel ();

			goCurInfoContent = Instantiate (pfInfoAction, transform);
			viewInfoAction = goCurInfoContent.GetComponent<ViewInfoAction> ();
			if (viewInfoAction == null) {
				Debug.LogError ("ERROR!  InfoAction prefab doesn't have a viewinfoaction component!");
			}
			viewInfoAction.Start ();
			viewInfoAction.SetModel (_mod);
		}
	}

	public void ClearPanel(){
		viewInfoAction = null;
		Destroy (goCurInfoContent);
		goCurInfoContent = null;
	}
		
	public void Start(){

	}
}
