using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContOptionsOverlay : MonoBehaviour {

    public bool bStarted = false;

    public enum Player1Type {
        HUMAN, AI
    };

    public enum Player2Type {
        HUMAN, AI
    };

    public enum TimerSelection {
        FAST, MEDIUM, INF
    };

    public ViewOptionsButton btnPlyr1Human;
    public ViewOptionsButton btnPlyr1AI;
    public ViewOptionsButton btnPlyr2Human;
    public ViewOptionsButton btnPlyr2AI;
    public ViewOptionsButton btnTimerFast;
    public ViewOptionsButton btnTimerMedium;
    public ViewOptionsButton btnTimerInf;

    public ViewOptionsButton btnRestart;

    public Subject subPlayer1SelectedInGroup = new Subject();
    public Subject subPlayer2SelectedInGroup = new Subject();
    public Subject subTimerSelectedInGroup = new Subject();

    public const float fDelayChooseActionFact = 5.0f;
    public const float fDelayChooseActionMedium = 30.0f;
    public const float fDelayChooseActionInf = 9999999.0f;

    public Vector3 v3OnScreen = new Vector3(0f, 0f, 0f);
    public Vector3 v3OffScreen = new Vector3(-100f, -100f, 0f);



    public void cbClickPlyr1Human(Object target, params object[] args) {


        subPlayer1SelectedInGroup.NotifyObs(target);
    }

    public void cbClickPlyr1AI(Object target, params object[] args) {


        subPlayer1SelectedInGroup.NotifyObs(target);
    }


    public void cbClickPlyr2Human(Object target, params object[] args) {


        subPlayer2SelectedInGroup.NotifyObs(target);
    }

    public void cbClickPlyr2AI(Object target, params object[] args) {


        subPlayer2SelectedInGroup.NotifyObs(target);
    }

    public void cbClickTimerFast(Object target, params object[] args) {

        ContTurns.fDelayChooseAction = fDelayChooseActionFact;

        subTimerSelectedInGroup.NotifyObs(target);
    }

    public void cbClickTimerMedium(Object target, params object[] args) {

        ContTurns.fDelayChooseAction = fDelayChooseActionMedium;

        subTimerSelectedInGroup.NotifyObs(target);
    }

    public void cbClickTimerInf(Object target, params object[] args) {

        ContTurns.fDelayChooseAction = fDelayChooseActionInf;

        subTimerSelectedInGroup.NotifyObs(target);
    }


    public void OnStart() {
        //Move the overlay onto the screen
        this.transform.position = v3OnScreen;

        //Initialize all of the button's action subscription and action groups
        btnPlyr1Human.subClick.Subscribe(cbClickPlyr1Human);
        btnPlyr1AI.subClick.Subscribe(cbClickPlyr1AI);
        btnPlyr2Human.subClick.Subscribe(cbClickPlyr2Human);
        btnPlyr2AI.subClick.Subscribe(cbClickPlyr2AI);
        btnTimerFast.subClick.Subscribe(cbClickTimerFast);
        btnTimerMedium.subClick.Subscribe(cbClickTimerMedium);
        btnTimerInf.subClick.Subscribe(cbClickTimerInf);

    }

    public void OnLeave() {
        //Move the overlay off of the screen
        this.transform.position = v3OffScreen;

        //Unsubscribe each button action
        btnPlyr1Human.subClick.UnSubscribe(cbClickPlyr1Human);
        btnPlyr1AI.subClick.UnSubscribe(cbClickPlyr1AI);
        btnPlyr2Human.subClick.UnSubscribe(cbClickPlyr2Human);
        btnPlyr2AI.subClick.UnSubscribe(cbClickPlyr2AI);
        btnTimerFast.subClick.UnSubscribe(cbClickTimerFast);
        btnTimerMedium.subClick.UnSubscribe(cbClickTimerMedium);
        btnTimerInf.subClick.UnSubscribe(cbClickTimerInf);

    }



    void Start () {
	    if(bStarted == false) {
            bStarted = true;

            //Initially hide the menu
            this.transform.position = v3OffScreen;

            
        }	
	}
}
