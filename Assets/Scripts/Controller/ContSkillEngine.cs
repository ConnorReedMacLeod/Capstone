﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO - change this name to ContGameEngine since we act upon more than just skill executions
public class ContSkillEngine : Singleton<ContSkillEngine> {

    public bool bStartedMatchLoop = false;

    public const int nFASTFORWARDTHRESHOLD = 3; //The number of stacked-up inputs beyond which we will fast forward through

    public Stack<Clause> stackClause = new Stack<Clause>();
    public Stack<Executable> stackExec = new Stack<Executable>();

    public Queue<Position> queueEmptiedPositions = new Queue<Position>(); //Track a list of positions that have been vacated that should be filled by new Chrs
                                                                          // (note that we shouldn't fill empty spots that would lead to use having more characters in
                                                                          //  play than the standard maximum)

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

        yield return ContTime.Get().WaitForSeconds(ContTime.fDelayStandard);
    }

    public bool IsMatchOver() {

        return Match.Get().matchresult.GetResult() != MatchResult.RESULT.UNFINISHED;
    }

    //Do any closing animations for the end of a match
    public IEnumerator CRCleanUpMatch() {

        Debug.Log("Cleaning up Match");

        yield return ContTime.Get().WaitForSeconds(ContTime.fDelayStandard);
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

        //Initially decide if we want to do any fast forwarding from early loaded input
        HandleFastForwarding();

        //Do any initial processing for beginning of match effects
        yield return ProcessStackUntilInputNeeded();

        //Keep processing effects while the match isn't finished
        while(!IsMatchOver()) {

            // At this point, we should have an input field that's been set up that needs to be filled out
            Debug.Assert(matchinputToFillOut != null);

            bool bNeedsLocalInput = false;

            //If we need input, let's check if we already have input waiting in our buffer for that input
            if(NetworkMatchReceiver.Get().IsCurMatchInputReady() == false) {

                // Now that input is needed by some player, check if we locally control that player
                if(NetworkMatchSetup.IsLocallyOwned(matchinputToFillOut.plyrActing.id)) {
                    bNeedsLocalInput = true;

                    //Let the input controller decide how it wants to submit its selections for this requested input
                    matchinputToFillOut.plyrActing.inputController.StartSelection(matchinputToFillOut);

                } else {
                    //If we don't locally control the player who needs to decide an input
                    DebugDisplay.Get().SetDebugText("Waiting for foreign input");
                }

                //Wait until we have input waiting for us in the network buffer
                while(NetworkMatchReceiver.Get().IsCurMatchInputReady() == false) {
                    //Keep spinning until we get the input we're waiting on

                    DebugDisplay.Get().SetDebugText("Waiting for input");
                    yield return null;
                }

                DebugDisplay.Get().SetDebugText("");

                //Do any cleanup that we need to do if we were waiting on input
                //TODO - figure out what needs to be done and under what circumstances - careful of potentially changing local input controllers
                if(bNeedsLocalInput == true) {
                    //If we were in charge of locally some selections, then we'll get our inputcontroller to cleanup anything it needs to
                    matchinputToFillOut.plyrActing.inputController.EndSelection(matchinputToFillOut);
                }

            }

            //Check if we should be master forwarding through our inputs if we have a bunch stacked up waiting to be processed (like from loading a log file or reconnecting)
            HandleFastForwarding();

            //At this point, we have an input in the buffer that we are able to process
            MatchInput matchinput = NetworkMatchReceiver.Get().GetCurMatchInput();

            //Make a record of which input we're going to be processing in our logs
            LogManager.Get().LogMatchInput(matchinput);

            //Clear out the matchinput we prompted to be filled out
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

    public void HandleFastForwarding() {
        //Check if we have a stacked up number of stored inputs that we need to plow through

        if(NetworkMatchReceiver.Get().HasNReadyInputs(nFASTFORWARDTHRESHOLD)) {
            ContTime.Get().SetAutoFastForward(true);
        } else {
            ContTime.Get().SetAutoFastForward(false);
        }
    }

    public void ResolveClause() {

        stackClause.Pop().Execute();

    }

    public static void PushClauses(IEnumerable<Clause> enumerClauses) {
        List<Clause> lstClauses = new List<Clause>(enumerClauses);

        //Push each Clause in sequence onto the stack, and ensure that the first
        // Clause in the sequence ends up at the top of the stack

        for(int i = lstClauses.Count - 1; i >= 0; i--) {
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

        Executable execResolving = stackExec.Pop();

        //Remove the Executable from the top of the stack and execute it
        yield return execResolving.Execute();

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

    //TODO - Go through any uses of this that should instead be Clause-wrapped
    public void AddExec(Executable exec) {

        //Push that executable onto the stack
        stackExec.Push(exec);

    }

    public void MaintainStateBasedActions() {
        //TODO:: This function

        //TODO:: Consider if we should check for replacement effects and triggers at resolve-time or at stack-pushing-time
    }

    public void SpawnTimer(float fDelay, string sLabel) {

        ViewTimer.Get().InitTimer(fDelay, sLabel);

    }

    public bool AreStacksEmpty() {
        return stackExec.Count == 0 && stackClause.Count == 0;
    }

    public IEnumerator ProcessStackUntilInputNeeded() {

        // Check if the flag has been raised to indicate we need player input to continue (and only
        //   continue if the match is still unfinished)
        while(matchinputToFillOut == null && IsMatchOver() == false) {
            // Keep processing game-actions until we need player input
            yield return ProcessStacks();
        }

    }

    //Executes the next executable (and will unpack clauses until it finds an executable)
    //  If no executables or clauses are left, then we'll handle the end-of-turnphase operations
    public IEnumerator ProcessStacks() {

        while(true) {
            while(stackExec.Count == 0) {
                //If we don't have any executables to process, then we have to unpack clauses until we do have an executable

                if(stackClause.Count > 0) {
                    //If we have a clause on our stack, then unpack that to hopefully add an executable to the stack
                    if(bDEBUGENGINE) Debug.Log("No Executables, so unpack a Clause");
                    ResolveClause();
                } else {
                    //If we have no clauses on our stack, then our stack is completely empty, so we've reached the end of this turn phase

                    //First, do a check to see if there's any dead characters - if so, we'll kill off the first death-flagged character and then reprocess the stack until
                    //  any death triggers have been resolved and we return back here to see if there's any other death-flagged characters to deal with
                    if(ContDeaths.Get().KillNextFlaggedDyingCharacter() == true) {

                        continue;
                    }

                    //If we get to this point, then we don't have any more characters to kill off, but we may have to react to any characters that have died earlier
                    // in this phase

                    //Let's check if the result of the match is complete now
                    Match.Get().matchresult = ContDeaths.Get().CheckMatchWinner();

                    if(Match.Get().matchresult.GetResult() != MatchResult.RESULT.UNFINISHED) {
                        if(bDEBUGENGINE) Debug.LogFormat("Match is over!  The result is: {0}", Match.Get().matchresult.GetResult());

                        //if the match is over, then we should return and gradually break out of the control flow of the main game-loop
                        yield break;
                    }

                    //If the game isn't over, then let's check if we have any remaining clean-up to do
                    if(queueEmptiedPositions.Count != 0) {
                        //If we have at least one position that has been marked to be filled, then we'll have to collect input to fill it

                        //Grab the oldest (first stacked) position to be filled
                        Position posEmptied = queueEmptiedPositions.Dequeue();

                        //And create an input request to have its owner fill that position
                        InputReplaceEmptyPos inputRequest = new InputReplaceEmptyPos(posEmptied.PlyrOwnedBy(), posEmptied);

                        //Queue up this input as needing to be resolved by the input-collected
                        matchinputToFillOut = inputRequest;

                        //Break out of this stack-processing since we don't currently have anything to process - answering this input will
                        //  give us some new executables to process
                        yield break;
                    } else {
                        //If we've processed any dying characters and there are no empty positions that still need to be filled,
                        // then we can finish wrapping up this phase of the turn - push the next turn phase's executable and we can process that
                        if(bDEBUGENGINE) Debug.Log("No Clauses, Executables, or Deaths so move to the next part of the turn");
                        ContTurns.Get().FinishedTurnPhase();
                    }
                }
            }

            //If we've gotten this far, then we know we have an executable to examine

            //Check if the executable on top of our stack has dealt with its pre-triggers/replacements yet
            if(stackExec.Peek().bPreTriggered) {
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
                AddExec(top);

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





    public void NotifyOfNewEmptyPosition(Position posEmptied) {

        Debug.Assert(posEmptied.chrOnPosition == null);

        //TODO - only add this position to our queue of emptied positions if this would mean we now have fewer
        //        chrs in play than we're supposed to
        Debug.LogError("TODO - only enque emptied positions if they'd put the team below their chr-minimum");
        if (posEmptied.positiontype != Position.POSITIONTYPE.BENCH) {
            queueEmptiedPositions.Enqueue(posEmptied);
        }

    }




    public override void Init() {

    }

}
