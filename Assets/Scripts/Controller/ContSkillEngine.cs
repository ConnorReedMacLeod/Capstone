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


    //The main loop that will process the effects of the game.  If it needs inputs, it will flag what
    //  it's waiting on and pull input from the network buffer to decide what action should be taken
    public IEnumerator CRMatchLoop() {

        //Do any animation processing that needs to be done before the match processing actually starts
        yield return StartCoroutine(CRPrepMatch());


        //Keep processing effects while the match isn't finished
        while (IsMatchOver()) {

            // Pass control over to the stack-processing loop until it needs player input to continue the simulation
            yield return ProcessStackUntilInputNeeded();

            //Check if we have input waiting for us in the network buffer
            while (NetworkDraftReceiver.Get().IsCurSelectionReady() == false) {
                //Keep spinning until we get the input we're waiting on

                WaitForMatchInput();
                yield return null;
            }
            //If we were waiting on input, then clean up the waiting process
            if (draftactionWaitingOn != null) EndWaitingOnDraftInput();

            //At this point, we have an input in the buffer that we are able to process
            DraftAction draftaction = GetNextDraftPhaseStep();
            CharType.CHARTYPE chartypeInput = NetworkDraftReceiver.Get().GetCurSelection();

            //Start a coroutine to process whichever event we need to execute
            if (draftaction.draftactionType == DraftAction.DRAFTACTIONTYPE.DRAFT) {
                //Draft the character
                yield return StartCoroutine(CRDraftChr(draftaction.iPlayer, chartypeInput));
            } else {
                //Ban the character
                yield return StartCoroutine(CRBanChr(draftaction.iPlayer, chartypeInput));
            }

            //Now that the draft/ban is complete and processed, increment our 'current' indices
            NetworkDraftReceiver.Get().FinishCurSelection();
            FinishDraftPhaseStep();
        }

        //Do any animation process that needs to be done before we leave the draft scene
        yield return StartCoroutine(CRCleanUpMatch());

        //Wrap up the draft phase
        FinishMatch();
        yield break;
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


    public IEnumerator ResolveExec() {

        if(bDEBUGENGINE) Debug.Log("Resolving an Executable of type" + stackExec.Peek().GetType().ToString());

        //Remove the Executable from the top of the stack and execute it
        yield return stackExec.Pop().Execute();

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


    //TODONOW - consider making this a coroutine - be needed forever for everything, but can serve as 'animations' to describe the effects that are happening
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

    public IEnumerator ProcessStackUntilInputNeeded() {

        // Check if the flag has been raised to indicate we need player input to continue
        while (todo) {
            // Keep processing game-actions until we need player input
            yield return ProcessStacks();
        }

    }

    public IEnumerator ProcessStacks() {

        while (true) {
            while (stackExec.Count == 0) {
                //If we don't have any executables to process, then we have to unpack clauses until we do have an executable

                if (stackClause.Count > 0) {
                    //If we have a clause on our stack, then unpack that to hopefully add an executable to the stack
                    if (bDEBUGENGINE) Debug.Log("No Executables, so unpack a Clause");
                    ResolveClause();
                } else {
                    //If we have no clauses on our stack, then our stack is completely empty, so we can push 
                    // a new executable for the next phase of the turn
                    if (bDEBUGENGINE) Debug.Log("No Clauses or Executables so move to the next part of the turn");
                    ContTurns.Get().FinishedTurnPhase();
                }
            }

            //If we've gotten this far, then we know we have an executable to examine

            //Check if the executable on top of our stack has dealt with its pre-triggers/replacements yet
            if (stackExec.Peek().bPreTriggered) {
                //If we've already dealt with all its pretriggers/replacements, then we're ready to actually evaluate and we can 
                //  exit our loop
                break;
            } else { 

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

                //Now we can continue our loop to process whatever executable is now at the top of the stack
            }

        }

        //If we've gotten this far, then we know we have an executable at the top of our stack and 
        //  we are ready to process it
        yield return ResolveExec();
        
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
