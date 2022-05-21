using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ManaPool))]
public class ViewManaPool : MonoBehaviour {

    bool bStarted;                          //Confirms the Start() method has executed

    public Text txtManaPhysicalUsable;
    public Text txtManaMentalUsable;
    public Text txtManaEnergyUsable;
    public Text txtManaBloodUsable;
    public Text txtManaEffortUsable;

    public Text txtManaPhysicalReserved;
    public Text txtManaMentalReserved;
    public Text txtManaEnergyReserved;
    public Text txtManaBloodReserved;
    public Text txtManaEffortReserved;

    public ManaPool mod;                   //reference to the player's mana model

    public Vector3 v3TopMiddle = new Vector3(0f, 2.85f, 0f);
    public Vector3 v3MiddleLeft = new Vector3(-4.59f, 0f, 0f);
    public Vector3 v3MiddleRight = new Vector3(4.59f, 0f, 0f);
    public Vector3 v3Offscreen = new Vector3(-1000f, -1000f, 0f);


    // Use this for initialization
    public void Start() {
        if(bStarted == false) {
            bStarted = true;

            mod.subManaChange.Subscribe(cbManaChange);

            Player.subAllInputTypeChanged.Subscribe(cbInputTypeChanged);

            DisplayAllMana();
        }
    }

    public void cbInputTypeChanged(Object target, params object[] args) {
        PositionPanel();
    }

    public void PositionPanel() {

        if (mod.plyr.inputController == null || mod.plyr.GetEnemyPlayer().inputController == null) {
            //If we haven't initialized an input controller for one of the players, then don't bother
            //  trying to reposition ourselves - when they're initialized, we'll be notified and can move then
            return;
        }

        //If both players are human
        if (mod.plyr.inputController.GetInputType() == LocalInputType.InputType.HUMAN && mod.plyr.GetEnemyPlayer().inputController.GetInputType() == LocalInputType.InputType.HUMAN) {
            //Then position the mana panels towards the sides of the screen
            if(mod.plyr.id == 0) {
                this.transform.localPosition = v3MiddleLeft;
            } else if(mod.plyr.id == 1) {
                this.transform.localPosition = v3MiddleRight;
            }
            //If we are the only human
        } else if(mod.plyr.inputController.GetInputType() == LocalInputType.InputType.HUMAN) {
            this.transform.localPosition = v3TopMiddle;

            //If both players are AI
        } else if(mod.plyr.inputController.GetInputType() == LocalInputType.InputType.AI && mod.plyr.GetEnemyPlayer().inputController.GetInputType() == LocalInputType.InputType.AI) {
            //Currently we'll put the left players mana on the screen
            if(mod.plyr.id == 0) {
                this.transform.localPosition = v3MiddleLeft;
            } else if(mod.plyr.id == 1) {
                this.transform.localPosition = v3MiddleRight;
            }

        } else {
            //Otherwise, put the panel offscreen
            this.transform.localPosition = v3Offscreen;
        }

    }

    public void UpdateManaText(Text _txtMana, int _nMana) {
        if(_nMana == 0) {
            _txtMana.text = "";
        } else {
            _txtMana.text = _nMana.ToString();
        }
    }
    

    public void DisplayAllMana() {

        if (mod.manaOwned == null) return;

        UpdateManaText(txtManaPhysicalUsable, mod.manaOwned[Mana.MANATYPE.PHYSICAL]);
        UpdateManaText(txtManaMentalUsable, mod.manaOwned[Mana.MANATYPE.MENTAL]);
        UpdateManaText(txtManaEnergyUsable, mod.manaOwned[Mana.MANATYPE.ENERGY]);
        UpdateManaText(txtManaBloodUsable, mod.manaOwned[Mana.MANATYPE.BLOOD]);
        UpdateManaText(txtManaEffortUsable, mod.manaOwned[Mana.MANATYPE.EFFORT]);
    }

    public void cbManaChange(Object target, params object[] args) {
        switch((Mana.MANATYPE)args[0]) {
        case Mana.MANATYPE.PHYSICAL:
            UpdateManaText(txtManaPhysicalUsable, mod.manaUsableToPay[Mana.MANATYPE.PHYSICAL]);
            UpdateManaText(txtManaPhysicalReserved, mod.manaReservedToPay[Mana.MANATYPE.PHYSICAL]);
            break;
        case Mana.MANATYPE.MENTAL:
            UpdateManaText(txtManaMentalUsable, mod.manaUsableToPay[Mana.MANATYPE.MENTAL]);
            UpdateManaText(txtManaMentalReserved, mod.manaReservedToPay[Mana.MANATYPE.MENTAL]);
            break;
        case Mana.MANATYPE.ENERGY:
            UpdateManaText(txtManaEnergyUsable, mod.manaUsableToPay[Mana.MANATYPE.ENERGY]);
            UpdateManaText(txtManaEnergyReserved, mod.manaReservedToPay[Mana.MANATYPE.ENERGY]);
            break;
        case Mana.MANATYPE.BLOOD:
            UpdateManaText(txtManaBloodUsable, mod.manaUsableToPay[Mana.MANATYPE.BLOOD]);
            UpdateManaText(txtManaBloodReserved, mod.manaReservedToPay[Mana.MANATYPE.BLOOD]);
            break;
        case Mana.MANATYPE.EFFORT:
            UpdateManaText(txtManaEffortUsable, mod.manaUsableToPay[Mana.MANATYPE.EFFORT]);
            UpdateManaText(txtManaEffortReserved, mod.manaReservedToPay[Mana.MANATYPE.EFFORT]);
            break;
        }

    }

}
