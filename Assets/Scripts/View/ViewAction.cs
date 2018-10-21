using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewAction : ViewInteractive {

    bool bStarted;                          //Confirms the Start() method has executed

    public int id;                              //The action's unique identifier
	public Action mod;                      		//The action's model

    //Textfields to display information
    public Text txtName;
    public Text txtType;
    public Text txtRecharge;
    public Text txtCooldown;
    public Text txtRemaining;

    public static Subject subAllClick = new Subject();
    public static Subject subAllStartHover = new Subject();
    public static Subject subAllStopHover = new Subject();

    public override void onMouseClick(params object[] args) {

        subAllClick.NotifyObs(this, args);

        base.onMouseClick(args);
    }

    public override void onMouseStartHover(params object[] args) {

        subAllStartHover.NotifyObs(this, args);

        base.onMouseStartHover(args);
    }

    public override void onMouseStopHover(params object[] args) {

        subAllStopHover.NotifyObs(this, args);

        base.onMouseStopHover(args);
    }


    //Let the Action button know which character and id it's representing
    public void SetModel (Action _mod){
		mod = _mod;
        DisplayAll();
    }

	public void Start(){
        if (bStarted == false)
        {
            bStarted = true;

            Chr.subAllStartSelect.Subscribe(cbChrSelected);
            Chr.subAllStartIdle.Subscribe(cbChrUnselected);
        }
	}

    public void cbChrSelected(Object target, params object[] args) {
        SetModel(((Chr)target).arActions[id]);
    }

    public void cbChrUnselected(Object target, params object[] args) {
        SetModel(null);
    }

    public void DisplayName(){
        if (mod == null){
            txtName.text = "";
        } else {
            txtName.text = mod.sName;
        }
    }

    public void DisplayCost() {
        //TODO: THIS FUNCTION
    }

    public void DisplayType() {
        if (mod == null) {
            txtType.text = "";
        } else {
            txtType.text = "[" + mod.type.ToString() + "]";
        }
    }

    public void DisplayRecharge() {
        if (mod == null) {
            txtRecharge.text = "";
        } else {
            txtRecharge.text = mod.nRecharge.ToString();
        }
    }

    public void DisplayCooldown() {
        if (mod == null) {
            txtCooldown.text = "";
        } else {
            txtCooldown.text = mod.nCd.ToString();
        }
    }

    public void DisplayRemaining() {
        if (mod == null) {
            txtRemaining.text = "";
        } else {
            //txtRemaining.text = "CD: " + mod.nCd.ToString();
            //TODO:  THIS PART OF THE FUNCTION
        }
    }

    public void DisplayAll() {
        DisplayName();
        DisplayCost();
        DisplayType();
        DisplayRecharge();
        DisplayCooldown();
        DisplayRemaining();
    }
}
