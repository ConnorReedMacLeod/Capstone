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


    //Let the Action button know which character and id it's representing
    public void SetModel (Action _mod){
		mod = _mod;
        DisplayAll();
    }

	public void Start(){
        if (bStarted == false)
        {
            bStarted = true;
            //Init();

            /* Match.Get().Start();
             for (int i = 0; i < Match.Get().nPlayers; i++) {
                 for (int j = 0; j < Match.Get().arChrs[1].Length; j++) {
                     Match.Get().arChrs[i][j].Subscribe(this);
                 }
             }*/
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

    //TODO:: Just link this in the prefab
    //Variable initialization
    public void Init()
    {
        Text[] arTextComponents = GetComponentsInChildren<Text>();

        for (int i = 0; i < arTextComponents.Length; i++)
        {

            switch (arTextComponents[i].name)
            {
                case "txtName":
                    txtName = arTextComponents[i];
                    break;

                case "txtType":
                    txtType = arTextComponents[i];
                    break;

                case "txtRecharge":
                    txtRecharge = arTextComponents[i];
                    break;

                case "txtCooldown":
                    txtCooldown = arTextComponents[i];
                    break;

                case "txtRemaining":
                    txtRemaining = arTextComponents[i];
                    break;

                default:
                    Debug.LogError("ERROR! Unrecognized Text component in ViewAction");
                    break;
            }
        }
        
    }

}
