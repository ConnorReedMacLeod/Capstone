using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ContDeaths : Singleton<ContDeaths> {


    public struct DeathTimestamp {
        public Timestamp timestampDeath;
        public Chr chr;

        public DeathTimestamp(Timestamp _timestampDeath, Chr _chr) {
            timestampDeath = _timestampDeath;
            chr = _chr;
        }
    }

    public Queue<DeathTimestamp> queueDyingChrs; //Tracks a list of all characters that have been flagged as dying (in order of their death 'timestamp')


    public override void Init() {
        queueDyingChrs = new Queue<DeathTimestamp>();
    }


    public void AddDyingChr(Chr chr) {

        Debug.LogFormat("Adding {0} as a dying character", chr);

        Timestamp curTimestamp = ContTimestamp.Get().GetCurrentTimestamp();

        //Record the death of this chr with the current timestamp at the time of death
        //  - we'll use this timestamp to later determine if 
        queueDyingChrs.Enqueue(new DeathTimestamp(curTimestamp, chr));

        //Store this death timestamp in the dead character themselves so they know at
        // what time they died
        chr.timestampDeath = curTimestamp;

        Debug.LogFormat("Potential death was at {0}", chr.timestampDeath);
    }

    //Find the first character that has been flagged as dying and that actually should die and push a death effect to transition them to a dead state
    // returns true/false if there are/aren't any dead characters
    public bool KillNextFlaggedDyingCharacter() {

        while(queueDyingChrs.Count != 0) {
            DeathTimestamp chrNextFlaggedDyingTimestamp = queueDyingChrs.Dequeue();

            //If this first character is indeed supposed to die
            // (and we're also double-checking that this timestamp we're checking is indeed the most recent death
            //   the character has faced - i.e., if the character dropped below 0, then went above 0, then dropped below 0 again,
            //   then we want to make sure we're only processing the second death to ensure the order of execution is correct)
            if(chrNextFlaggedDyingTimestamp.chr.IsDying()
                && chrNextFlaggedDyingTimestamp.chr.timestampDeath.nTimestamp == chrNextFlaggedDyingTimestamp.timestampDeath.nTimestamp) {
                //Then let's push a death executable onto the stack to kill the character

                ContSkillEngine.PushSingleExecutable(new ExecKillFlaggedDyingChr(null, chrNextFlaggedDyingTimestamp.chr));

                return true; //Return true that we have indeed found a character to kill
            }
        }
        //We can guarantee that our queue will be completely cleared out if we're reached this point

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
