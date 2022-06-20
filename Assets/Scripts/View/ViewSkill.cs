using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewSkill : ViewInteractive {

    bool bStarted;                          //Confirms the Start() method has executed

    public Skill mod;                      		//The skill's model

    public GameObject goIcon;

    public const string sHIDDENSKILLICONPATH = "Images/Chrs/imgHeadshotMask";

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
        DisplayAll(ShouldHide(mod));
    }


    //Let the Skill button know which skill it's representing
    public void SetModel(Skill _mod) {

        if (mod != null) {
            //If we we're previously showing an skill, then unsubscribe from it
            mod.subSkillChange.UnSubscribe(cbSkillChanged);
        }

        mod = _mod;

        if (mod != null) {

            DisplayAll(ShouldHide(mod));

            //If we're now subscribed to an actual skill, then subscribe to it
            mod.subSkillChange.Subscribe(cbSkillChanged);
        }
    }

    public override void Start() {
        if (bStarted == false) {
            bStarted = true;

            base.Start();
        }
    }

    public static bool ShouldHide(Skill skill) {
        Debug.LogFormat("Skill to maybe hide is {0}", skill);

        //If we've not using the hidden skills rule, then definitely don't hide any skills
        if (ContOptionsOverlay.Get().bHiddenSkillsRule == false) return false;

        //If the skill doesn't need to be hidden, then don't hide it
        if (skill.pbHidden.Get() == false) return false;

        //If the skill should be publicly hidden, then we just need to check if
        // we locally own the skill (and control it as a human) and thus don't need to hide it
        if (skill.chrOwner.IsLocallyOwned() == true && skill.chrOwner.plyrOwner.inputController.GetInputType() == LocalInputType.InputType.HUMAN) return false;

        return true;
    }

    public void DisplayIcon(bool bHiddenSkill) {
        if(mod == null) return;


        //By default, set the spritepath to be a blank hidden icon (TODO - update this graphic)
        string sSprPath = sHIDDENSKILLICONPATH;

        //And if we don't need to hide it, then update it to the relevent icon
        if (bHiddenSkill == false) {
            sSprPath = "Images/Chrs/" + mod.chrOwner.sName + "/img" + mod.sName;
        }

        LibView.AssignSpritePathToObject(sSprPath, goIcon);
    }



    public void DisplayName(bool bHiddenSkill) {
        if(mod == null) {
            txtName.text = "";
        } else if (bHiddenSkill == true) {
            txtName.text = "???";
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

    public void DisplayCost(bool bHiddenSkill) {
        if(mod == null || bHiddenSkill == true) {
            txtType.text = "";
        } else {
            txtCost.text = mod.manaCost.ToPrettyString();
        }
    }

    public void DisplayType(bool bHiddenSkill) {
        if(mod == null) {
            txtType.text = "";
        } else if (bHiddenSkill == true) {
            txtType.text = "??";
        } else {
            txtType.text = "[" + mod.typeUsage.getName() + "]";
        }
    }

    public void DisplayCurCooldown(bool bHiddenSkill) {
        if(mod == null || mod.skillslot.nCooldown == 0 || bHiddenSkill == true) {
            txtCurCooldown.text = "";
        } else {
            txtCurCooldown.text = mod.skillslot.nCooldown.ToString();
        }
    }

    public void DisplayFatigue(bool bHiddenSkill) {
        if(mod == null) {
            txtFatigue.text = "";
        } else if (bHiddenSkill == true) {
            txtFatigue.text = "??";
        } else {
            txtFatigue.text = mod.nFatigue.ToString();
        }
    }


    public void DisplayCooldown(bool bHiddenSkill) {
        if (mod == null) {
            txtCooldown.text = "";
        } else if (bHiddenSkill == true) {
            txtCooldown.text = "??";
        } else {
            txtCooldown.text = mod.nCooldownInduced.ToString();
        }
    }

    public void DisplayAll(bool bHiddenSkill) {

        DisplayName(bHiddenSkill);
        DisplayCost(bHiddenSkill);
        DisplayType(bHiddenSkill);
        DisplayCurCooldown(bHiddenSkill);
        DisplayCooldown(bHiddenSkill);
        DisplayIcon(bHiddenSkill);
            
    }
}
