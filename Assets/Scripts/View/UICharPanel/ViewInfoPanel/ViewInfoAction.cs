using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ViewInfoAction : MonoBehaviour {

	bool bStarted;                          //Confirms the Start() method has executed

	public Text txtName;
	public Text txtCost;

	public Text txtType;
	public Text txtFatigue;
	public Text txtCooldown;
	public Text txtCharges;

	public Text txtDescription1;
	public Text txtDescription2;
	public Text txtDescription3;

	public Action mod;                   //Action model

    public Subject subInfoActionUpdate = new Subject();

	// Use this for initialization
	public void Start() {
		if (bStarted == false) {
			bStarted = true;
			//Init();
			//Unscale ();

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
            txtType.text = "";
        } else {
            string sPhys = new string('1', mod.parCost.Get()[(int)Mana.MANATYPE.PHYSICAL]);
            string sMent = new string('2', mod.parCost.Get()[(int)Mana.MANATYPE.MENTAL]);
            string sEnrg = new string('3', mod.parCost.Get()[(int)Mana.MANATYPE.ENERGY]);
            string sBld = new string('4', mod.parCost.Get()[(int)Mana.MANATYPE.BLOOD]);
            string sEfrt = new string('5', mod.parCost.Get()[(int)Mana.MANATYPE.EFFORT]);

            txtCost.text = sPhys + sMent + sEnrg + sBld + sEfrt;
        }
    }

	public void DisplayType() {
		if (mod == null) {
			txtType.text = "";
		} else {
			txtType.text = mod.type.getName();
		}
	}

	public void DisplayFatigue() {
		if (mod == null) {
			txtFatigue.text = "";
		} else {
			txtFatigue.text = "FTG: " + mod.nFatigue.ToString();
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

	public void DisplayDescription1() {
		if (mod == null) {
			txtDescription1.text = "";
		} else {
			txtDescription1.text = mod.sDescription1;
		}
	}

	public void DisplayDescription2() {
		if (mod == null) {
			txtDescription2.text = "";
		} else {
			txtDescription2.text = mod.sDescription2;
		}
	}

	public void DisplayDescription3() {
		if (mod == null) {
			txtDescription3.text = "";
		} else {
			txtDescription3.text = mod.sDescription3;
		}
	}

	public void DisplayAll(){
		DisplayName ();
		DisplayCost ();
		DisplayType ();
		DisplayFatigue ();
		DisplayCooldown ();
		DisplayCharges ();
		DisplayDescription1 ();
		DisplayDescription2 ();
		DisplayDescription3 ();
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

        DisplayAll();
    }


}
