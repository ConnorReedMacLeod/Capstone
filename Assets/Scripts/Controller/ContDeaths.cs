using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ContDeaths : Singleton<ContDeaths> {


    public Queue<Chr> queueDyingChrs; //Tracks a list of all characters that have been flagged as dying (in order of their death 'timestamp')


    public override void Init() {
        queueDyingChrs = new Queue<Chr>();
    }


    public void AddDyingChr(Chr chr) {

        if(queueDyingChrs.Contains(chr)) {
            Debug.LogFormat("{0} is already earlier in the queue of dying characters", chr);
            return;
        }

        queueDyingChrs.Enqueue(chr);
    }


    //Find the first character that has been flagged as dying and that actually should die and push a death effect to transition them to a dead state
    // returns true/false if there are/aren't any dead characters
    public bool KillNextFlaggedDyingCharacter() {

        while(queueDyingChrs.Count != 0) {
            Chr chrNextFlagged = queueDyingChrs.Dequeue();

            //If this first character is indeed supposed to die
            if(chrNextFlagged.IsDying()) {
                //Then let's push a death executable onto the stack to kill the character

                ContSkillEngine.PushSingleExecutable(new ExecKillFlaggedDyingChr(null, chrNextFlagged));

                return true; //Return true that we have indeed found a character to kill
            }
        }

        //If we've exited the loop, then we didn't find any characters that were supposed to die, so just return false
        return false;
    }


    //Checks if the conditions have been met for (at least) one player to lose the game
    // Return a MatchResult encapsulating the status of the match
    public MatchResult CheckMatchWinner() {

        int nPlayer0DeadChrs = ChrCollection.Get().GetDeadChrsOwnedBy(Match.Get().arPlayers[0]).Count;
        int nPlayer1DeadChrs = ChrCollection.Get().GetDeadChrsOwnedBy(Match.Get().arPlayers[1]).Count;

        if(nPlayer0DeadChrs >= Match.NCHARACTERLIVESPERTEAM) {

            if(nPlayer1DeadChrs >= Match.NCHARACTERLIVESPERTEAM) {

                Debug.LogError("Since both teams have lost enough characters to lose, the result is a draw");

                return new MatchResultDraw();

            } else {
                //If player 0 has enough dead chrs to lose, but player 1 doesn't, then player 1 wins
                return new MatchResultDecisive(1);
            }

        } else if(nPlayer1DeadChrs >= Match.NCHARACTERLIVESPERTEAM) {
            //If player 0 doesn't have enough dead chrs to lose, but player 1 does, then player 0 wins
            return new MatchResultDecisive(0);
        }

        //If neither player has lost enough chrs, then the match isn't over yet
        return new MatchResultUnfinished();

    }
}
