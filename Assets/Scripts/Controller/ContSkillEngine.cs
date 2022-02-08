using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContSkillEngine : Singleton<ContSkillEngine> {

    public bool bStartedMatchLoop = false;

    public Stack<Clause> stackClause = new Stack<Clause>();
    public Stack<Executable> stackExec = new Stack<Executable>();

    public GameObject pfTimer;
    public ViewTimer viewTimerCur;

    public const bool bDEBUGENGINE = false;

    public MatchInput matchinputToFillOut;  //A reference to the match input that needs to be filled out before we can progress
                                            //  with the rest of the match simulation (may be filled out locally, or we can ignore it
                                            //  if a foreign player is supposed to fill it out)

    public void StartMatchLoop() {
        if(bStartedMatchLoop == true) return; //If we were already started, no need to start again
        bStartedMatchLoop = true;

        StartCoroutine(CRMatchLoop());
    }


    public IEnumerator CRPrepMatch() {

        Debug.Log("Prepping Match");

        yield return new WaitForSeconds(1.0f);
    }

    public bool IsMatchOver() {

        Debug.Log("IsMatchOver - not yet implemented");
        return false;
    }

    //Do any closing animations for the end of a match
    public IEnumerator CRCleanUpMatch() {

        Debug.Log("Cleaning up Match");

        yield return new WaitForSeconds(1.0f);
    }

    //Do any saving of results/rewards and move to a new scene
    public void FinishMatch() {
        Debug.Log("Finishing Match");
    }

    //The main loop that will process the effects of the game.  If it needs inputs, it will flag what
    //  it's waiting on and pull input from the network buffer to decide what action should be taken
    public IEnumerator CRMatchLoop() {

        //Do any animation processing that needs to be done before the match processing actually starts
        yield return StartCoroutine(CRPrepMatch());

        //Do any initial processing for beginning of match effects
        yield return ProcessStackUntilInputNeeded();

        //Keep processing effects while the match isn't finished
        while (!IsMatchOver()) {

            // At this point, we should have an input field that's been set up that needs to be filled out
            Debug.Assert(matchinputToFillOut != null);

            bool bNeedsLocalInput = false;

            //If we need input, let's check if we already have input waiting in our buffer for that input
            if (NetworkMatchReceiver.Get().IsCurMatchInputReady() == false) {

                // Now that input is needed by some player, check if we locally control that player
                if (NetworkMatchSetup.IsLocallyOwned(matchinputToFillOut.iPlayerActing)) {
                    bNeedsLocalInput = true;
                    //Let the match input prepare to start gathering manual input 
                    matchinputToFillOut.StartManualInputProcess();
                } else {
                    //If we don't locally control the player who needs to decide an input
                    Debug.Log("Waiting for foreign input");
                }

                //Wait until we have input waiting for us in the network buffer
                while (NetworkMatchReceiver.Get().IsCurMatchInputReady() == false) {
                    //Keep spinning until we get the input we're waiting on

                    Debug.Log("Waiting for input");
                    yield return null;
                }

                //Do any cleanup that we need to do if we were waiting on input
                //TODO - figure out what needs to be done and under what circumstances - careful of potentially changing local input controllers
                if(bNeedsLocalInput == true) {
                    //Have the match input let the local input controller know that we're done with gathering input
                    matchinputToFillOut.EndManualInputProcess();
                }

            }

            //At this point, we have an input in the buffer that we are able to process
            MatchInput matchinput = NetworkMatchReceiver.Get().GetCurMatchInput();

            //Clear out the matchinput we prompting to be filled out
            matchinputToFillOut = null;

            //Process that match input by deferring to its execute method
            yield return matchinput.Execute();

            //The execute method should have pushed new executables/clauses onto the stack, so we can process them
            // Pass control over to the stack-processing loop until it needs player input to continue the simulation
            yield return ProcessStackUntilInputNeeded();

            //Since we're done processing all the effects that may need access to the most recent input, we can advance to the next needed input
            NetworkMatchReceiver.Get().FinishCurMatchInput();
        }

        //Do any animation process that needs to be done before we leave the match scene
        yield return StartCoroutine(CRCleanUpMatch());

        //Do any fill wrap-up for the match
        FinishMatch();
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

    //Note, this isn't a coroutine, so if you want to delay until the timer is finished, just do a WaitForSeconds after spawning the timer
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
        while (matchinputToFillOut == null) {
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

   

    public override void Init() {
        
    }

}
