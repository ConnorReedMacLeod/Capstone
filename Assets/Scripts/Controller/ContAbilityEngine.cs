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

    public const bool bDEBUGENGINE = false;

    public static ContAbilityEngine instance;

    public static ContAbilityEngine Get() {
        if (instance == null) {
            GameObject go = GameObject.FindGameObjectWithTag("Controller");
            if (go == null) {
                Debug.LogError("ERROR! NO OBJECT HAS A Controller TAG!");
            }
            instance = go.GetComponent<ContAbilityEngine>();
            if (instance == null) {
                Debug.LogError("ERROR! Controller TAGGED OBJECT DOES NOT HAVE A ContAbilityEngine COMPONENT!");
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

        if(bDEBUGENGINE) Debug.Log("Pushing a Clause");

        stackClause.Push(clause);

    }


    public void ResolveExec() {

        //Remove the Executable from the top of the stack (Assume no replacements need to take effect)
        stackExec.Pop().Execute();

    }


    public Executable ResolveFullReplacements(Executable execToResolve) {

        //Loop through each effect that could replace us
        for(int i=0; i<execToResolve.GetFullReplacements().Count; i++) {

            //If we have already applied this effect, then move on to the next replacement
            if (execToResolve.GetFullReplacements()[i].bHasReplaced) continue;

            //If the replacement effect shouldn't take effect, then also move on to the next replacement
            if (!execToResolve.GetFullReplacements()[i].shouldReplace(execToResolve)) continue;

            //If we haven't moved on by this point, then we should implement this replacement effect
            //then recurse on this new executable to see if it needs to be replaced
            return ResolveFullReplacements(execToResolve.GetFullReplacements()[i].ApplyReplacement(execToResolve));

        }

        //If we never account a relevent fullreplacement, then just return the exec we started with
        return execToResolve;
    }


    public Executable ResolveReplacements(Executable execBaseExecutable) {

        Executable execToResolve = execBaseExecutable;

        List<Replacement> lstReplacements = execToResolve.GetReplacements(); 
        //This should stay constant since our executable type isn't changing, so neither is the static lstReplacements

        //Loop through each effect that could replace us
        for (int i = 0; i < lstReplacements.Count; i++) {

            //If we have already applied this effect, then move on to the next replacement
            if (lstReplacements[i].bHasReplaced) continue;

            //If the replacement effect shouldn't take effect, then also move on to the next replacement
            if (!lstReplacements[i].shouldReplace(execToResolve)) continue;

            //If we haven't moved on by this point, then we should implement this replacement effect
            // unlike the full replacements, we don't need to completely recurse - just change the current executable
            execToResolve = lstReplacements[i].ApplyReplacement(execToResolve);

        }

        //return whatever executable we're left with at this point
        return execToResolve;
    }


    public void AddExec(Executable exec) {

        //Check for any replacement effects

        //Initially, we reset the cycle-checking flags for each registered replacement effect
        Replacement.ResetReplacedFlags();

        //Resolve any full replacement effects (so we settle on a single type of executable)
        exec = ResolveFullReplacements(exec);

        //Now we modify that executable as much as necessary
        exec = ResolveReplacements(exec);

        //TODO:: Consider if we should check for replacement effects and triggers at resolve-time or at stack-pushing-time

        stackExec.Push(exec);

        //After this has been pushed on the stack, cycle through any pre-triggers
        //to see what should be put on top of this effect
        exec.GetPreTrigger().NotifyObs(null, exec);

    }

    public void MaintainStateBasedActions() {
        //TODO:: This function

        //TODO:: Consider if we should check for replacement effects and triggers at resolve-time or at stack-pushing-time
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
            if (bDEBUGENGINE) Debug.Log("Resolving an Executable");
            ResolveExec();
            return;
        }

        //Check statebased actions
        MaintainStateBasedActions();

        //Next, check if there's any clauses to process
        if(stackClause.Count > 0) {
            if (bDEBUGENGINE) Debug.Log("No Executables, so unpack a Clause");
            ResolveClause();

            //Then recurse to find something new we can process
            ProcessStacks();
            return;
        }

        //Then we have nothing left to process
        //So ask the ContTurn to add the executable for the next phase in the turn

        if (bDEBUGENGINE) Debug.Log("No Clauses or Executables so move to the next part of the turn");
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
