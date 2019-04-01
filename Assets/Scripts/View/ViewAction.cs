using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewAction : ViewInteractive {

    bool bStarted;                          //Confirms the Start() method has executed

    public int id;                              //The action's unique identifier
	public Action mod;                      		//The action's model

    //Textfields to display information
    public Text txtCost;
    public Text txtName;
    public Text txtType;
    public Text txtCurCooldown;
    public Text txtCooldown;
    public Text txtFatigue;

    public GameObject goIcon;

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

    public void cbAbilityChanged(Object target, params object[] args) {
        DisplayAll();
    }


    //Let the Action button know which action it's representing
    public void SetModel(Action _mod) {

        if (mod != null) {
            //If we we're previously showing an ability, then unsubscribe from it
            mod.subAbilityChange.UnSubscribe(cbAbilityChanged);
        }

        mod = _mod;
        DisplayAll();

        if (mod != null) {
            //If we're now subscribed to an actual ability, then subscribe to it
            mod.subAbilityChange.Subscribe(cbAbilityChanged);
        }
    }

	public void Start(){
        if (bStarted == false)
        {
            bStarted = true;
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
        if (mod == null) {
            txtType.text = "";
        } else {
            int[] arCost = mod.parCost.Get();
            string sPhys = new string('1', arCost[(int)Mana.MANATYPE.PHYSICAL]);
            string sMent = new string('2', arCost[(int)Mana.MANATYPE.MENTAL]);
            string sEnrg = new string('3', arCost[(int)Mana.MANATYPE.ENERGY]);
            string sBld = new string('4', arCost[(int)Mana.MANATYPE.BLOOD]);
            string sEfrt = new string('5', arCost[(int)Mana.MANATYPE.EFFORT]);

            txtCost.text = sPhys + sMent + sEnrg + sBld + sEfrt;
        }
    }

    public void DisplayType() {
        if (mod == null) {
            txtType.text = "";
        } else {
            txtType.text = "[" + mod.type.getName() + "]";
        }
    }

    public void DisplayCurCooldown() {
        if (mod == null || mod.nCurCD == 0) {
            txtCurCooldown.text = "";
        } else {
            txtCurCooldown.text = mod.nCurCD.ToString();
        }
    }

    public void DisplayFatigue() {
        if (mod == null) {
            txtFatigue.text = "";
        } else {
            txtFatigue.text = mod.nFatigue.ToString();
        }
    }


    public void DisplayCooldown() {
        if (mod == null) {
            txtCooldown.text = "";
        } else {
            txtCooldown.text = mod.nCd.ToString();
        }
    }

    public void DisplayIcon() {
        string sImgPath = "";

        if(mod != null) {
            sImgPath = "Images/Chrs/" + mod.chrSource.sName + "/img" + mod.sName;
        }

        Sprite sprIcon = Resources.Load(sImgPath, typeof(Sprite)) as Sprite;
        goIcon.GetComponent<SpriteRenderer>().sprite = sprIcon;
		
	}


    public void DisplayAll() {
        DisplayName();
        DisplayCost();
        DisplayType();
        DisplayCurCooldown();
        DisplayCooldown();
        DisplayIcon();
    }
}
