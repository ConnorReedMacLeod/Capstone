using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ViewInfoAction : Observer {

	bool bStarted;                          //Confirms the Start() method has executed

	public Text txtName;
	public Text txtCost;

	public Text txtType;
	public Text txtRecharge;
	public Text txtCooldown;
	public Text txtCharges;

	public Text txtDescription;
	public Text txtExtraDescription;

	public Action mod;                   //Action model

	// Use this for initialization
	public void Start() {
		if (bStarted == false) {
			bStarted = true;
			Init();
			Unscale ();

			//Reposition to be at the origin
			transform.localPosition = Vector3.zero;
		}
	}

	public void DisplayName() {
		if (mod == null) {
			txtName.text = "";
		} else {
			txtName.text = mod.sName;
		}
	}

	public string CostToString(int []_cost){
		if (_cost.Length != Mana.nManaTypes) {
			Debug.Log ("ERROR!  GIVEN MANA COST HAS WRONG NUMBER OF VALUES!");
			return "";
		} else {
			string sPhys = new string(LibText.PrepSymbol("P"), _cost [0]);
			string sMen = new string(LibText.PrepSymbol ("M"), _cost [1]);
			string sEnergy = new string(LibText.PrepSymbol ("E"), _cost [2]);
			string sBld = new string(LibText.PrepSymbol("B"), _cost [3]);
			string sEff = new string(LibText.PrepSymbol ("O"), _cost [4]);

			return sPhys + sMen + sEnergy + sBld + sEff;
		}
	}

	public void DisplayCost() {
		if (mod == null) {
			txtCost.text = "";
		} else {
			txtCost.text = CostToString (mod.arCost);
		}
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
			txtRecharge.text = "RC: " + mod.nRecharge.ToString();
		}
	}

	public void DisplayCooldown() {
		if (mod == null) {
			txtCooldown.text = "";
		} else {
			txtCooldown.text = "CD: " + mod.nCd.ToString();
		}
	}

	public void DisplayCharges() {
		if (mod == null) {
			txtCharges.text = "";
		} else if (mod.bCharges == false) {
			txtCharges.text = "";
		}else {
			txtCharges.text = "Charges: [" + mod.nCurCharges.ToString() + "]";
		}
	}

	public void DisplayDescription() {
		if (mod == null) {
			txtDescription.text = "";
		} else {
			txtDescription.text = mod.sDescription;
		}
	}

	public void DisplayExtraDescription() {
		if (mod == null) {
			txtExtraDescription.text = "";
		} else {
			txtExtraDescription.text = mod.sExtraDescription;
		}
	}

	public void DisplayAll(){
		DisplayName ();
		DisplayCost ();
		DisplayType ();
		DisplayRecharge ();
		DisplayCooldown ();
		DisplayCharges ();
		DisplayDescription ();
		DisplayExtraDescription ();
	}

	override public void UpdateObs(string eventType, Object target, params object[] args) {

		switch (eventType) {
		//TODO:: Consider adding in field-specific update types if only one field needs updating

		case Notification.InfoActionUpdate:
			DisplayAll ();
			break;

		default:
			break;
		}
	}

	//Variable initialization
	public void Init() {
		Text[] arTextComponents = GetComponentsInChildren<Text>();

		for (int i = 0; i < arTextComponents.Length; i++) {

			switch (arTextComponents[i].name) {
			case "txtName":
				txtName = arTextComponents[i];
				break;

			case "txtCost":
				txtCost = arTextComponents[i];
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

			case "txtCharges":
				txtCharges = arTextComponents [i];
				break;

			case "txtDescription":
				txtDescription = arTextComponents[i];
				break;

			case "txtExtraDescription":
				txtExtraDescription = arTextComponents[i];
				break;

			default:
				Debug.LogError ("ERROR! Unrecognized Text component in ViewInfoAction");
				break;
			}
		}

		UpdateObs (Notification.InfoActionUpdate, null);
	}

	//Undoes the image and border scaling set by the parent
	public void Unscale(){
		transform.localScale = new Vector3
			(transform.localScale.x / transform.parent.localScale.x,
				transform.localScale.y / transform.parent.localScale.y,
				transform.localScale.z / transform.parent.localScale.z);
	}

	public void SetModel(Action _mod) {
		mod = _mod;

		//TODO:: Consider if observing the owner of the action to 
		// dynamically update the description text makes sense
		//mod.Subscribe(this);

		UpdateObs (Notification.InfoActionUpdate, null);
	}


}
