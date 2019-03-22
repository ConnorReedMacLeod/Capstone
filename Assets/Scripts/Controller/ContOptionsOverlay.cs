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



    public void cbSelectedOptionInGroup(Object target, params object[] args) {

    }

    public void OnStart() {
        //Initialize all of the button's action subscription and action groups


    }

    public void OnLeave() {
        //Unsubscribe each button action

    }



    void Start () {
	    if(bStarted == false) {
            bStarted = true;


        }	
	}
}
