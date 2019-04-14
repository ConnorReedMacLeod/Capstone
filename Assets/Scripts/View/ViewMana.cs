using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Mana))]
public class ViewMana : MonoBehaviour {

	bool bStarted;                          //Confirms the Start() method has executed

	public Text txtManaPhysical;
	public Text txtManaMental;
	public Text txtManaEnergy;
	public Text txtManaBlood;

	public Text txtManaEffort;

	public Text txtManaEffortPhysical;
	public Text txtManaEffortMental;
	public Text txtManaEffortEnergy;
	public Text txtManaEffortBlood;

	public Mana mod;                   //Character model

    public Vector3 v3TopMiddle = new Vector3(0f, 2.85f, 0f);
    public Vector3 v3MiddleLeft = new Vector3(-4.59f, 0f, 0f);
    public Vector3 v3MiddleRight = new Vector3(4.59f, 0f, 0f);
    public Vector3 v3Offscreen = new Vector3(100f, 100f, 0f);


    // Use this for initialization
    public void Start() {
		if (bStarted == false) {
			bStarted = true;
			InitModel();

            mod.subManaChange.Subscribe(cbManaChange);
            mod.subManaPoolChange.Subscribe(cbManaPoolChange);

            Player.subAllInputTypeChanged.Subscribe(cbInputTypeChanged);

        }
	}

    public void cbInputTypeChanged(Object target, params object[] args) {
        PositionPanel();
    }

    public void PositionPanel() {

        //If both players are human
        if (mod.plyr.curInputType == Player.InputType.HUMAN && mod.plyr.GetEnemyPlayer().curInputType == Player.InputType.HUMAN) {
            //Then position the mana panels towards the sides of the screen
            if (mod.plyr.id == 0) {
                this.transform.position = v3MiddleLeft;
            } else if (mod.plyr.id == 1) {
                this.transform.position = v3MiddleRight;
            }
           //If we are the only human
        } else if (mod.plyr.curInputType == Player.InputType.HUMAN) {
            this.transform.position = v3TopMiddle;

            //If both players are AI
        }else if (mod.plyr.curInputType == Player.InputType.AI && mod.plyr.GetEnemyPlayer().curInputType == Player.InputType.AI) {
            //Currently we'll put the left players mana on the screen
            if (mod.plyr.id == 0) {
                this.transform.position = v3TopMiddle;
            } else if (mod.plyr.id == 1) {
                this.transform.position = v3Offscreen;
            }
           
        } else {
            //Otherwise, put the panel offscreen
            this.transform.position = v3Offscreen;
        }

    }

	public void InitModel() {
		mod = GetComponent<Mana>();
	}

    public void DisplayMana(Text _txtMana, int _nMana) {
        if (_nMana == 0) {
            _txtMana.text = "";
        } else { 
            _txtMana.text = _nMana.ToString();
        }
    }

    public void cbManaChange(Object target, params object[] args) {
        switch ((Mana.MANATYPE)args[0]) {
            case Mana.MANATYPE.PHYSICAL:
                DisplayMana(txtManaPhysical, mod.arMana[(int)Mana.MANATYPE.PHYSICAL]);
                break;
            case Mana.MANATYPE.MENTAL:
                DisplayMana(txtManaMental, mod.arMana[(int)Mana.MANATYPE.MENTAL]);
                break;
            case Mana.MANATYPE.ENERGY:
                DisplayMana(txtManaEnergy, mod.arMana[(int)Mana.MANATYPE.ENERGY]);
                break;
            case Mana.MANATYPE.BLOOD:
                DisplayMana(txtManaBlood, mod.arMana[(int)Mana.MANATYPE.BLOOD]);
                break;
        }
    }

    public void cbManaPoolChange(Object target, params object[] args) {
        DisplayMana(txtManaEffort, mod.nManaPool);

        switch ((Mana.MANATYPE)args[0]) {
            case Mana.MANATYPE.PHYSICAL:
                DisplayMana(txtManaEffortPhysical, mod.arManaPool[(int)Mana.MANATYPE.PHYSICAL]);
                break;
            case Mana.MANATYPE.MENTAL:
                DisplayMana(txtManaEffortMental, mod.arManaPool[(int)Mana.MANATYPE.MENTAL]);
                break;
            case Mana.MANATYPE.ENERGY:
                DisplayMana(txtManaEffortEnergy, mod.arManaPool[(int)Mana.MANATYPE.ENERGY]);
                break;
            case Mana.MANATYPE.BLOOD:
                DisplayMana(txtManaEffortBlood, mod.arManaPool[(int)Mana.MANATYPE.BLOOD]);
                break;
        }
    }

    
}
