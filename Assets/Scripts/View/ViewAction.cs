using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO:: Consider if not making this an observer is okay
[RequireComponent (typeof(MouseHandler))]
public class ViewAction : Observer {

    bool bStarted;                          //Confirms the Start() method has executed

    public int id;                              //The action's unique identifier
	public Action mod;                      		//The action's model
	public MouseHandler mousehandler;

    //Textfields to display information
    public Text txtName;
    public Text txtType;
    public Text txtRecharge;
    public Text txtCooldown;
    public Text txtRemaining;

    //Let the Action button know which character and id it's representing
	public void SetModel (Action _mod){
		mod = _mod;
        DisplayAll();
    }

	public void Start(){
        if (bStarted == false)
        {
            bStarted = true;
            Init();
            InitMouseHandler();

            Match.Get().Start();
            for (int i = 0; i < Match.Get().nPlayers; i++) {
                for (int j = 0; j < Match.Get().arChrs[1].Length; j++) {
                    Match.Get().arChrs[i][j].Subscribe(this);
                }
            }
        }
	}

	public void InitMouseHandler(){
		mousehandler = GetComponent<MouseHandler> ();
		mousehandler.SetOwner (this);

        mousehandler.SetNtfClick(Notification.ClickAct);
		mousehandler.SetNtfStartHover (Notification.ActStartHover);
		mousehandler.SetNtfStopHover (Notification.ActStopHover);
	}

    /* TODO:: REMOVE THESE, SINCE MOUSEHANDLER SHOULD TAKE CARE OF THIS
    //Notifies application when the Action is clicked
    public void OnMouseDown(){
		Controller.Get().NotifyObs(Notification.ClickAct, this, id);
	}	

	public void OnMouseEnter(){
		Controller.Get ().NotifyObs (Notification.ActStartHover, this, id);
	}

	public void OnMouseExit(){
		Controller.Get ().NotifyObs (Notification.ActStopHover, this, id);
	}
    */


    override public void UpdateObs(string eventType, Object target, params object[] args)
    {
        switch (eventType) {
            //TODO:: Consider adding in field-specific update types if only one field needs updating

            case Notification.ChrSelected:
            //case Notification.ActionUpdate:
                SetModel(((Chr)target).arActions[id]);
                break;

            default:
                break;
        }
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

        UpdateObs(Notification.ActionUpdate, null);
    }

}
