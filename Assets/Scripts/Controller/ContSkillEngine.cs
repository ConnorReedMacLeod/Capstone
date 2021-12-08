using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContSkillEngine : Singleton<ContSkillEngine> {

    public bool bAutoTurns = false;

    public Stack<Clause> stackClause = new Stack<Clause>();
    public Stack<Executable> stackExec = new Stack<Executable>();

    public GameObject pfTimer;
    public ViewTimer viewTimerCur;

    public const bool bDEBUGENGINE = false;

    public void StartAutoProcessingStacks() {
        if(bAutoTurns == true) return; //If we were already started, no need to start again
        bAutoTurns = true;

        if(bAutoTurns) {
            Debug.Log("Going to next event in " + 1.0f);

            ContTime.Get().Invoke(1.0f, AutoProcessStacks);
        }
    }
    public void AutoProcessStacks() {

        if(!bAutoTurns) {
            //Then we must have switched to manual turns while waiting for this event,
            //so don't actually execute anything automatically
            return;
        }

        ProcessStacks();
    }

    public void cbManualExecuteEvent(Object target, params object[] args) {
        bAutoTurns = false;

        Debug.Log("********************************************");
        //Check if there's any stack to process
        if(AreStacksEmpty()) {
            //If the stacks are empty, then manual execution of the phase just means
            // submitting a signal to the master to let it know we're done with the phase
            Debug.Log("Manually finishing a turn phase");
            ContTurns.Get().FinishedTurnPhase();
        } else {
            //There's still effects to process, so process the contents of the stacks just once
            Debug.Log("Manually processing stacks");
            ProcessStacks();
        }

    }

    public void ResolveClause() {

        stackClause.Pop().Execute();

    }

    public static void PushClauses(IEnumerable<Clause> enumerClauses) {
        List<Clause> lstClauses = new List<Clause>(enumerClauses);

        //Push each Clause in sequence onto the stack, and ensure that the first
        // Clause in the sequence ends up at the top of the stack

        for (int i = lstClauses.Count - 1; i >= 0; i--) {
            PushSingleClause(lstClauses[i]);
        }

    }

    public static void PushSingleClause(Clause clause) {

        if(bDEBUGENGINE) Debug.Log("Pushing a Clause");

        Get().stackClause.Push(clause);

    }

    public static void PushExecutables(List<Executable> lstExecs) {

        //Push each Executable in sequence onto the stack, and ensure that the 
        // first executable in the sequence ends up at the top of the stack
        for(int i = lstExecs.Count - 1; i >= 0; i--) {
            PushSingleExecutable(lstExecs[i]);
        }
    }

    public static void PushSingleExecutable(Executable exec) {

        if(bDEBUGENGINE) Debug.Log("Pushing an Executable of type " + exec.GetType().ToString());

        Get().stackExec.Push(exec);
    }


    public void ResolveExec() {

        if(bDEBUGENGINE) Debug.Log("Resolving an Executable of type" + stackExec.Peek().GetType().ToString());

        //Remove the Executable from the top of the stack and execute it
        stackExec.Pop().Execute();

    }


    public Executable ResolveFullReplacements(Executable execToResolve) {

        //Loop through each effect that could replace us
        for(int i = 0; i < execToResolve.GetFullReplacements().Count; i++) {

            //If we have already applied this effect, then move on to the next replacement
            if(execToResolve.GetFullReplacements()[i].bHasReplaced) continue;

            //If the replacement effect shouldn't take effect, then also move on to the next replacement
            if(!execToResolve.GetFullReplacements()[i].shouldReplace(execToResolve)) continue;

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
        for(int i = 0; i < lstReplacements.Count; i++) {

            //If we have already applied this effect, then move on to the next replacement
            if(lstReplacements[i].bHasReplaced) continue;

            //If the replacement effect shouldn't take effect, then also move on to the next replacement
            if(!lstReplacements[i].shouldReplace(execToResolve)) continue;

            //If we haven't moved on by this point, then we should implement this replacement effect
            // unlike the full replacements, we don't need to completely recurse - just change the current executable
            execToResolve = lstReplacements[i].ApplyReplacement(execToResolve);

        }

        //return whatever executable we're left with at this point
        return execToResolve;
    }


    public void AddExec(Executable exec) {

        stackExec.Push(exec);

    }

    public void MaintainStateBasedActions() {
        //TODO:: This function

        //TODO:: Consider if we should check for replacement effects and triggers at resolve-time or at stack-pushing-time
    }

    public void SpawnTimer(float fDelay, string sLabel) {
        GameObject goTimer = Instantiate(pfTimer, Match.Get().transform); //TIMER SPAWN POSITION
        ViewTimer viewTimer = goTimer.GetComponent<ViewTimer>();
        if(viewTimer == null) {
            Debug.LogError("ERROR - pfTimer doesn't have a viewTimer component");
        }
        viewTimer.InitTimer(fDelay, sLabel);


        //Check if we should delete the previous timer
        if(viewTimerCur != null) {
            //Debug.Log("Deleting previous timer for " + viewTimerCur.sLabel);
            Destroy(viewTimerCur.gameObject);
        }
        viewTimerCur = viewTimer;

    }

    public bool AreStacksEmpty() {
        return stackExec.Count == 0 && stackClause.Count == 0;
    }

    public void ProcessStacks() {

        //First, check if there's any executables to process
        if(stackExec.Count > 0) {

            //If we're seeing this executable for the first time and have
            //to process replacement and pre-trigger effects
            if(!stackExec.Peek().bPreTriggered) {

                //Debug.Log("Performing Replacement effects and Pre-Triggers");

                //Pop off the top element
                Executable top = stackExec.Pop();

                //Check for any replacement effects
                //Initially, we reset the cycle-checking flags for each registered replacement effect
                Replacement.ResetReplacedFlags();

                //Resolve any full replacement effects (so we settle on a single type of executable)
                top = ResolveFullReplacements(top);

                //Now we modify that executable as much as necessary
                top = ResolveReplacements(top);

                //Push the modified executable back onto the stack at the top
                stackExec.Push(top);

                //Now we can push all of the pre-triggers onto the stack
                top.GetPreTrigger().NotifyObs(null, top);

                //Set our flag so that we don't pre-trigger this effect again
                top.bPreTriggered = true;

                //Now recurse so that we can process whatever effect should come next
                if(bDEBUGENGINE) Debug.Log("Recursing on ProcessStacks after resolving replacements");
                ProcessStacks();

            } else {
                //at this point, we can actually evaluate this executable
                ResolveExec();

            }

            return;
        }

        //Debug.Log("Processing stack with no executables");

        //Check statebased actions
        MaintainStateBasedActions();

        //Next, check if there's any clauses to process
        if(stackClause.Count > 0) {
            if(bDEBUGENGINE) Debug.Log("No Executables, so unpack a Clause");
            ResolveClause();

            //Then recurse to find something new we can process

            if(bDEBUGENGINE) Debug.Log("Recursing on ProcessStacks after unpacking a clause");
            ProcessStacks();
            return;
        }

        //Then we have nothing left to process, so we need to move to the next phase to put its executable on the stack
        if(bDEBUGENGINE) Debug.Log("No Clauses or Executables so move to the next part of the turn");
        ContTurns.Get().FinishedTurnPhase();

        //Now recurse here so that we can process the next executable that has been put on our stack for the next
        //  phase of the turn
        ProcessStacks();

        if(bDEBUGENGINE) Debug.Log("Reached the end of ProcessStacks");

    }

    //Other classes can call this to invoke the ProcessStack method after a delay
    public void InvokeProcessStack(float fDelay, string sLabel, bool bCancelInvoke) {
        if(bAutoTurns) {

            if(fDelay > 0) {
                //Check if we need to spawn a timer

                SpawnTimer(fDelay, sLabel);
            }
            if(bCancelInvoke == false) {

                if(bDEBUGENGINE) Debug.Log("Calling autoprocessstacks with a delay after finishing processing a previous executable");
                ContTime.Get().Invoke(fDelay, AutoProcessStacks);
            }
        } else {
            //Debug.Log("Manually executing " + sLabel);
            //Then we're doing manual execution - still spawn a quick timer to show what we're processing right now

            if(fDelay == 0.0f) {
                //If there wouldn't be any delay on evaluating anyway, then just immediately 
                //Process the next event immediately without spawning a timer


                if(bDEBUGENGINE) Debug.Log("Immediately calling ProcessStacks since there's no delay between executions");
                ProcessStacks();
            } else {
                //If there is a delay, then just spawn the timer and wait for the user to click to move
                // on to evaluating the next event later

                SpawnTimer(1.0f, sLabel);
            }


        }


    }

    public override void Init() {
        
    }

}
