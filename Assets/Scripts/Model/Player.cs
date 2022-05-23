using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour {

    bool bStarted;

    public int id;

    public LocalInputType inputController;

    public ManaPool manapool;
    public ManaCalendar manacalendar;

    public static Subject subAllInputTypeChanged = new Subject(Subject.SubType.ALL);
    public static Subject subAllPlayerLost = new Subject(Subject.SubType.ALL);

    public int GetTargettingId() {
        return id;
    }

    public void SetID(int _id) {
        id = _id;
    }

    public void SetInputType(LocalInputType.InputType inputtype) {

        switch(inputtype) {

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

        if(inputController != null) {

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

    public void InitManaPool() {

        if(id == 0) {
            manapool = Match.Get().manapool0;
        } else if(id == 1){
            manapool = Match.Get().manapool1;
        }

        manapool.SetPlayer(this);

    }

    public void SpawnManaCalendar() {

        if(id == 0) {
            manacalendar = Match.Get().manaCalendar0;
        }else if(id == 1) {
            manacalendar = Match.Get().manaCalendar1;
        }

        manacalendar.SetPlayer(this);
    }

    // Use this for initialization
    public void Start() {

        if(bStarted == false) {
            bStarted = true;

            InitManaPool();
            SpawnManaCalendar();

        }
    }

}
