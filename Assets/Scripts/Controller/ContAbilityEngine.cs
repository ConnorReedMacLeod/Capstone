using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContAbilityEngine : MonoBehaviour {

    public bool bStarted = false;
    public bool bAutoTurns = false;

    public Stack<Clause> stackClause = new Stack<Clause>();
    public Stack<Executable> stackExec = new Stack<Executable>();

    public GameObject pfTimer;
    public ViewTimer viewTimerCur;

    public static ContAbilityEngine instance;

    public static ContAbilityEngine Get() {
        if (instance == null) {
            GameObject go = GameObject.FindGameObjectWithTag("ContAbilityEngine");
            if (go == null) {
                Debug.LogError("ERROR! NO OBJECT HAS A ContAbilityEngine TAG!");
            }
            instance = go.GetComponent<ContAbilityEngine>();
            if (instance == null) {
                Debug.LogError("ERROR! ContAbilityEngine TAGGED OBJECT DOES NOT HAVE A ContAbilityEngine COMPONENT!");
            }
            instance.Start();
        }
        return instance;
    }

    public void cbAutoProcessStacks(Object target, params object[] args) {
        if (bAutoTurns == true) return; //If the button is already pressed
        bAutoTurns = true;

        if (bAutoTurns) {
            Debug.Log("Going to next event in " + 2.0f);

            Invoke("AutoProcessStacks", 2.0f);
        }
    }
    public void AutoProcessStacks() {

        if (!bAutoTurns) {
            //Then we must have switched to manual turns while waiting for this event,
            //so don't actually execute anything automatically
            return;
        }

        ProcessStacks();
    }

    public void cbManualExecuteEvent(Object target, params object[] args) {
        bAutoTurns = false;

        ProcessStacks();
    }

    public void ResolveClause() {

        //Maybe just check for replacement effects when things are put on the stack?   Might make things easier

        stackClause.Pop().Execute();

    }


    public void AddClause(Clause clause) {

        //Check for any replacements or triggers or something?

        stackClause.Push(clause);

    }


    public void ResolveExec() {

        //Tentative:
        //Check if any replacement need to replace the top of the stack
        //Check pre-executing events need to trigger

        //Remove the Executable from the top of the stack (once any replacements have been cleared)
        stackExec.Pop().Execute();

    }

    public void AddExec(Executable exec) {

        //Check for any replacement effects

        stackExec.Push(exec);

    }

    public void MaintainStateBasedActions() {
        //TODO:: This function
    }

    public void SpawnTimer(float fDelay, string sLabel) {
        GameObject goTimer = Instantiate(pfTimer, Match.Get().transform);
        ViewTimer viewTimer = goTimer.GetComponent<ViewTimer>();
        if (viewTimer == null) {
            Debug.LogError("ERROR - pfTimer doesn't have a viewTimer component");
        }
        viewTimer.InitTimer(fDelay, sLabel);

        /* TODO:: Uncomment this if everything else works
        //Check if we should delete the previous timer
        if(viewTimerCur != null) {
            Destroy(viewTimerCur.gameObject);
        }
        viewTimerCur = viewTimer;
        */
    }

    public void ProcessStacks() {

        //First, check if there's any executables to process
        if(stackExec.Count > 0) {
            Debug.Log("Resolving an Executable");
            ResolveExec();
            return;
        }

        //Check statebased actions
        MaintainStateBasedActions();

        //Next, check if there's any clauses to process
        if(stackClause.Count > 0) {
            Debug.Log("No Executables, so unpack a Clause");
            ResolveClause();

            //Then recurse to find something new we can process
            ProcessStacks();
            return;
        }

        //Then we have nothing left to process
        //So ask the ContTurn to add the executable for the next phase in the turn

        Debug.Log("No Clauses or Executables so move to the next part of the turn");
        ContTurns.Get().HandleTurnPhase();

        //And recurse to process the newly added Executable
        ProcessStacks();

    }

    //Other classes can call this to invoke the ProcessStack method after a delay
    public void InvokeProcessStack(float fDelay, string sLabel) {
        if (bAutoTurns) {

            if (fDelay > 0) {
                //Check if we need to spawn a timer

                SpawnTimer(fDelay, sLabel);
            }

            Invoke("AutoProcessStacks", fDelay);
        } else {
            Debug.Log("Manually executing " + sLabel);
            //Then we're doing manual execution - still spawn a quick timer to show what we're processing right now
            SpawnTimer(1.0f, sLabel);
            
        }
        

    }

    public void Start() {
        if (bStarted) return;
        bStarted = true;


        ViewAutoTurnsButton.subAllAutoExecuteEvent.Subscribe(cbAutoProcessStacks);
        ViewManualTurnsButton.subAllManualExecuteEvent.Subscribe(cbManualExecuteEvent);
    }

}
