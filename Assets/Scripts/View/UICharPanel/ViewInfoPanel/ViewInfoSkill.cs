using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ViewInfoSkill : MonoBehaviour {

    bool bStarted;                          //Confirms the Start() method has executed

    public GameObject goIcon;

    public Text txtName;
    public Text txtCost;

    public Text txtType;
    public Text txtFatigue;
    public Text txtCooldown;
    public Text txtCharges;

    public Text txtDescription1;
    public Text txtDescription2;
    public Text txtDescription3;

    public Skill mod;                   //Skill model

    public Subject subInfoSkillUpdate = new Subject();

    // Use this for initialization
    public void Start() {
        if(bStarted == false) {
            bStarted = true;
            //Init();
            //Unscale ();

            //Reposition to be at the origin
            transform.localPosition = Vector3.zero;
        }
    }

    public void DisplayIcon(bool bHidden) {

        if(mod == null) {
            return;
        }

        string sSprPath = ViewSkill.sHIDDENSKILLICONPATH;

        if(bHidden == false) {
            sSprPath = "Images/Chrs/" + mod.chrOwner.sName + "/img" + mod.sName;
        }

        LibView.AssignSpritePathToObject(sSprPath, goIcon);
    }

    public void DisplayName(bool bHidden) {
        if(mod == null) {
            txtName.text = "";
        } else if (bHidden == true) {
            txtName.text = "??";
        } else {
            txtName.text = mod.sDisplayName;
        }
    }
    

    public void DisplayCost(bool bHidden) {
        if(mod == null || bHidden == true) {
            txtCost.text = "";
        } else {
            txtCost.text = mod.manaCost.ToPrettyString();
        }
    }

    public void DisplayType(bool bHidden) {
        if(mod == null) {
            txtType.text = "";
        } else if (bHidden == true) {
            txtType.text = "??";
        } else {
            txtType.text = mod.typeUsage.getName();
        }
    }

    public void DisplayFatigue(bool bHidden) {
        if(mod == null) {
            txtFatigue.text = "";
        } else if (bHidden == true) {
            txtFatigue.text = "FTG: ??";
        } else {
            txtFatigue.text = "FTG: " + mod.nFatigue.ToString();
        }
    }

    public void DisplayCooldown(bool bHidden) {
        if (mod == null) {
            txtCooldown.text = "";
        } else if (bHidden == true) {
            txtCooldown.text = "CD: ??";
        } else {
            txtCooldown.text = "CD: " + mod.nCooldownInduced.ToString();
        }
    }

    public void DisplayCharges(bool bHidden) {
        if(mod == null || bHidden == true) {
            txtCharges.text = "";
        } else if(mod.bCharges == false) {
            txtCharges.text = "";
        } else {
            txtCharges.text = "Charges: [" + mod.nCurCharges.ToString() + "]";
        }
    }

    public void DisplayDescription1(bool bHidden) {
        if (mod == null) {
            txtDescription1.text = "";
        } else if (bHidden == true) {
            txtDescription1.text = "???";
        } else if(mod.lstSkillClauses.Count() <= 0) {
            txtDescription1.text = "";
        } else {
            txtDescription1.text = mod.lstSkillClauses[0].GetDescription();
        }
    }

    public void DisplayDescription2(bool bHidden) {
        if(mod == null || bHidden == true || mod.lstSkillClauses.Count() <= 1) {
            txtDescription2.text = "";
        } else {
            txtDescription2.text = mod.lstSkillClauses[1].GetDescription();
        }
    }

    public void DisplayDescription3(bool bHidden) {
        if(mod == null || bHidden == true || mod.lstSkillClauses.Count() <= 2) {
            txtDescription3.text = "";
        } else {
            txtDescription3.text = mod.lstSkillClauses[2].GetDescription();
        }
    }

    public void DisplayAll(bool bHidden) {

        DisplayIcon(bHidden);
        DisplayName(bHidden);
        DisplayCost(bHidden);
        DisplayType(bHidden);
        DisplayFatigue(bHidden);
        DisplayCooldown(bHidden);
        DisplayCharges(bHidden);
        DisplayDescription1(bHidden);
        DisplayDescription2(bHidden);
        DisplayDescription3(bHidden);

    }

    //Undoes the image and border scaling set by the parent
    public void Unscale() {
        transform.localScale = new Vector3
            (transform.localScale.x / transform.parent.localScale.x,
                transform.localScale.y / transform.parent.localScale.y,
                transform.localScale.z / transform.parent.localScale.z);
    }

    public void SetModel(Skill _mod) {
        mod = _mod;
        bool bHiddenSkill = ViewSkill.ShouldHide(mod);

        Debug.LogFormat("Displaying {0}, hidden = {1}", _mod.sDisplayName, bHiddenSkill);
        DisplayAll(ViewSkill.ShouldHide(mod));
    }


}
