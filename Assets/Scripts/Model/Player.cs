using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour {

    bool bStarted;

    public int id;
    public const int MAXCHRS = 3;
    public const int MAXPLAYERS = 2;
    public Chr[] arChr;
    public int nChrs;

    public static Player[] arAllPlayers;
    public GameObject pfManaPanel;

    public LocalInputType inputController;

    public Mana mana;

    public static Subject subAllInputTypeChanged = new Subject(Subject.SubType.ALL);
    public static Subject subAllPlayerLost = new Subject(Subject.SubType.ALL);

    public InputType curInputType;

    public enum InputType {
        NONE, HUMAN, AI
    };

    public List<Chr> GetActiveChrs() {
        return arChr.ToList<Chr>();
    }

    public int GetTargettingId() {
        return id;
    }

    public static Player GetTargetByIndex(int ind) {
        Debug.Log(arAllPlayers);
        Debug.Log("Asked for index " + ind + " with a arAllPlayers length of " + arAllPlayers.Length);
        return arAllPlayers[ind];
    }

    public static void RegisterPlayer(Player plyr) {
        if(arAllPlayers == null) {
            arAllPlayers = new Player[MAXPLAYERS];
        }

        Debug.Assert(plyr.id < MAXPLAYERS, "Can't ask for id " + plyr.id + " when MAXPLAYERS = " + MAXPLAYERS);

        arAllPlayers[plyr.id] = plyr;
    }

    public void SetID(int _id) {
        id = _id;
    }

    public void SetInputType(InputType inputType) {

        //If we already have an input Controller, then delete it
        if(inputController != null) {

            Destroy(inputController);
        }

        curInputType = inputType;

        switch(inputType) {
        case InputType.AI:
            //Then we want a script to control this player's selection
            inputController = gameObject.AddComponent<LocalInputScripted>();

            //Let the controller know which player its representing
            inputController.SetOwner(this);

            LocalInputScripted.SetRandomSkills((LocalInputScripted)inputController);

            break;

        case InputType.HUMAN:
            //Then we want the player to control this player's selection
            inputController = gameObject.AddComponent<LocalInputHuman>();

            //Let the controller know which player its representing
            inputController.SetOwner(this);

            break;

        case InputType.NONE:
            //Then no inputcontroller needs to be present for this player
            inputController = null;

            break;
        }


        subAllInputTypeChanged.NotifyObs(this, curInputType);
    }


    //Get a refernce to the enemy player
    public Player GetEnemyPlayer() {
        if(id == 0) {
            return Match.Get().arPlayers[1];
        } else {
            return Match.Get().arPlayers[0];
        }
    }

    // Use this for initialization
    public void Start() {

        if(bStarted == false) {
            bStarted = true;

            RegisterPlayer(this);

            arChr = new Chr[MAXCHRS];

            GameObject manaPanel = Instantiate(pfManaPanel, Match.Get().transform);
            mana = manaPanel.GetComponent<Mana>();

            mana.SetPlayer(this);

            //TODO: Change this, all this, to work with networking
            if(id == 0) {
                manaPanel.transform.position = new Vector3(0f, 2.85f, -0.4f);
            } else {
                //move it offscreen for now
                manaPanel.transform.position = new Vector3(100f, 100f, -0.4f);
            }

        }
    }

}
