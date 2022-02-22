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

    public ManaPool manapool;

    public static Subject subAllInputTypeChanged = new Subject(Subject.SubType.ALL);
    public static Subject subAllPlayerLost = new Subject(Subject.SubType.ALL);

    public List<Chr> GetActiveChrs() {
        return arChr.ToList<Chr>();
    }

    public int GetTargettingId() {
        return id;
    }

    public static Player GetTargetByIndex(int ind) {
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

    public void SetInputType(LocalInputType.InputType inputtype) {

        switch (inputtype) {

            case LocalInputType.InputType.NONE:
                inputController = null;
                break;

            case LocalInputType.InputType.HUMAN:
                inputController = new LocalInputHuman();
                break;

            case LocalInputType.InputType.AI:
                inputController = new LocalInputAI();
                break;

            case LocalInputType.InputType.SCRIPTED:
                inputController = new LocalInputScripted();
                break;
        }

        if (inputController != null) {

            inputController.SetOwner(this);
        }

        subAllInputTypeChanged.NotifyObs(this, inputtype);
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
            manapool = manaPanel.GetComponent<ManaPool>();

            manapool.SetPlayer(this);

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
