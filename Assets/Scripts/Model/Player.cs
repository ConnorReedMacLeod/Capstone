using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour{

	bool bStarted;

	public int id;
	public const int MAXCHRS = 3;
	public const int MAXPLAYERS = 2;
	public Chr[] arChr;
	public Chr.CHARTYPE[] arChrTypeSelection;
	public int nChrs;

    public int iBlocker; // the index of the currently selected blocker

    public static Player[] arAllPlayers;
	public GameObject pfManaPanel;

    public InputAbilitySelection inputController;

	public Mana mana;

    public static Subject subAllInputTypeChanged = new Subject(Subject.SubType.ALL);
    public static Subject subAllPlayerLost = new Subject(Subject.SubType.ALL);

    public InputType curInputType;

    public enum InputType {
        HUMAN, AI
    };


    public int GetTargettingId() {
        return id;
    }

    public static Player GetTargetByIndex(int ind) {
        return arAllPlayers[ind];
    }

    public static void RegisterPlayer(Player plyr) {
        if (arAllPlayers == null) {
            arAllPlayers = new Player[Player.MAXCHRS];
        }

        arAllPlayers[plyr.id] = plyr;
    }

    public void setChrs(){
        //placeholder until character selection is available

        nChrs = 3;

        if (id == 0) {
            arChrTypeSelection[0] = Chr.CHARTYPE.FISCHER;
            arChrTypeSelection[1] = Chr.CHARTYPE.RAYNE;
            arChrTypeSelection[2] = Chr.CHARTYPE.SOHPIDIA;
            

        } else {
            arChrTypeSelection[0] = Chr.CHARTYPE.SAIKO;
            arChrTypeSelection[1] = Chr.CHARTYPE.PITBEAST;
            arChrTypeSelection[2] = Chr.CHARTYPE.KATARINA;
             
             
            

        }
	}

	public void SetID(int _id){
		id = _id;
	}

    public void SetInputType(InputType inputType) {

        //If we already have an input Controller, then delete it
        if (inputController != null) {
            
            Destroy(inputController);
        }

        curInputType = inputType;

        switch (inputType) {
            case InputType.AI:
                //Then we want a script to control this player's selection
                inputController = gameObject.AddComponent<InputScripted>();
                InputScripted.SetRandomAbilities((InputScripted)inputController);

                break;

            case InputType.HUMAN:
                //Then we want the player to control this player's selection
                inputController = gameObject.AddComponent<InputHuman>();

                break;

        }

        //Let the controller know which player its representing
        inputController.SetOwner(this);
        
        subAllInputTypeChanged.NotifyObs(this, curInputType);
    }

    public void SetDefaultBlocker() {

        SetBlocker(ContTurns.Get().GetNextToActOwnedBy(this));

    }

    //Add an alternate signature for the function
    public void SetBlocker(Chr _chrBlocker) {
        SetBlocker(_chrBlocker.id);
    }

   public void SetBlocker(int _iBlocker) {

        Debug.Assert(arChr[_iBlocker] != null, "Assigned a blocker as a character that doesn't exist");
        if(iBlocker == _iBlocker) {
            Debug.Log("Then this character is already the blocker");
            return;
        }

        //TODO:: Make this more sophisticated
        if (iBlocker != -1) {
            arChr[iBlocker].ChangeBlocker(false);
        }

        iBlocker = _iBlocker;

        arChr[iBlocker].ChangeBlocker(true);
   
    }

    public Chr GetBlocker() {
        Debug.Assert(arChr[iBlocker] != null, "No blocker assigned to player " + id);
        return arChr[iBlocker];
    }

    //Get a refernce to the enemy player
    public Player GetEnemyPlayer() {
        if (id == 0) {
            return Match.Get().arPlayers[1];
        } else {
            return Match.Get().arPlayers[0];
        }
    }


    // Use this for initialization
    public void Start () {

		if (bStarted == false) {
			bStarted = true;

            arChr = new Chr[MAXCHRS];
			arChrTypeSelection = new Chr.CHARTYPE[MAXCHRS];

			GameObject manaPanel = Instantiate(pfManaPanel, Match.Get().transform);
			mana = manaPanel.GetComponent<Mana>();

			mana.SetPlayer (this);

			//TODO: Change this, all this, to work with networking
			if (id == 0) {
				manaPanel.transform.position = new Vector3(0f, 2.85f, -0.4f);
			} else {
				//move it offscreen for now
				manaPanel.transform.position = new Vector3(100f, 100f, -0.4f);
			}

		}
	}

}
