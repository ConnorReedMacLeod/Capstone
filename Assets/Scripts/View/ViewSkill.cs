using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewSkill : ViewInteractive {

    bool bStarted;                          //Confirms the Start() method has executed

    public Skill mod;                      		//The skill's model

    public GameObject goIcon;

    //Textfields to display information
    public Text txtCost;
    public Text txtName;
    public Text txtType;
    public Text txtCurCooldown;
    public Text txtCooldown;
    public Text txtFatigue;

    public static Subject subAllClick = new Subject(Subject.SubType.ALL);
    public static Subject subAllStartHover = new Subject(Subject.SubType.ALL);
    public static Subject subAllStopHover = new Subject(Subject.SubType.ALL);

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

    public void cbSkillChanged(Object target, params object[] args) {
        DisplayAll();
    }


    //Let the Skill button know which skill it's representing
    public void SetModel(Skill _mod) {

        if(mod != null) {
            //If we we're previously showing an skill, then unsubscribe from it
            mod.subSkillChange.UnSubscribe(cbSkillChanged);
        }

        mod = _mod;
        DisplayAll();

        if(mod != null) {
            //If we're now subscribed to an actual skill, then subscribe to it
            mod.subSkillChange.Subscribe(cbSkillChanged);
        }
    }

    public override void Start() {
        if(bStarted == false) {
            bStarted = true;

            base.Start();
        }
    }


    public void DisplayIcon() {
        if(mod == null) return;

        string sSprPath = "Images/Chrs/" + mod.chrOwner.sName + "/img" + mod.sName;

        Sprite sprIcon = Resources.Load(sSprPath, typeof(Sprite)) as Sprite;

        Debug.Assert(sprIcon != null, "Could not find specificed sprite: " + sSprPath);

        goIcon.GetComponent<SpriteRenderer>().sprite = sprIcon;
    }

    public void DisplayName() {
        if(mod == null) {
            txtName.text = "";
        } else {
            txtName.text = mod.sDisplayName;
        }
    }

    public static string GetEncodedManaCostText(ManaCost manaCost) {

        Mana manaCostFinal = manaCost.pManaCost.Get();
        string sPhys = new string('1', manaCostFinal[Mana.MANATYPE.PHYSICAL]);
        string sMent = new string('2', manaCostFinal[Mana.MANATYPE.MENTAL]);
        string sEnrg = new string('3', manaCostFinal[Mana.MANATYPE.ENERGY]);
        string sBld = new string('4', manaCostFinal[Mana.MANATYPE.BLOOD]);
        string sEfrt = new string('5', manaCostFinal[Mana.MANATYPE.EFFORT]);

        string sEncodedManaCost = sPhys + sMent + sEnrg + sBld + sEfrt;

        if(manaCost.bXCost) {
            sEncodedManaCost += "X";
        }

        return sEncodedManaCost;
    }

    public void DisplayCost() {
        if(mod == null) {
            txtType.text = "";
        } else {
            txtCost.text = GetEncodedManaCostText(mod.manaCost);
        }
    }

    public void DisplayType() {
        if(mod == null) {
            txtType.text = "";
        } else {
            txtType.text = "[" + mod.type.getName() + "]";
        }
    }

    public void DisplayCurCooldown() {
        if(mod == null || mod.skillslot.nCooldown == 0) {
            txtCurCooldown.text = "";
        } else {
            txtCurCooldown.text = mod.skillslot.nCooldown.ToString();
        }
    }

    public void DisplayFatigue() {
        if(mod == null) {
            txtFatigue.text = "";
        } else {
            txtFatigue.text = mod.nFatigue.ToString();
        }
    }


    public void DisplayCooldown() {
        if(mod == null) {
            txtCooldown.text = "";
        } else {
            txtCooldown.text = mod.nCooldownInduced.ToString();
        }
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
