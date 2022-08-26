using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ContDeaths : Singleton<ContDeaths> {


    public List<Chr> lstDyingChrs; //Tracks a list of all characters that have been flagged as dying (in order of their death 'timestamp')


    public override void Init() {
        lstDyingChrs = new List<Chr>();
    }


    public void AddDyingChr(Chr chr) {

        if(lstDyingChrs.Contains(chr)) {
            Debug.LogFormat("{0} is already in the list of dying characters", chr);
            return;
        }

        lstDyingChrs.Add(chr);
    }

    public void RemoveDyingChr(Chr chr) {

        if(lstDyingChrs.Contains(chr) == false) {
            Debug.LogFormat("{0} are not yet in the list of dying characters", chr);
            return;
        }

        lstDyingChrs.Remove(chr);
    }


    //Go through all chrs that have been flagged as dying and transition them (in order) to a dead state
    // returns true/false if there are/aren't any dead characters
    public bool KillFlaggedDyingChrs() {

        bool bFoundDeadChr = false;

        for(int i = 0; i < lstDyingChrs.Count; i++) {

            if(lstDyingChrs[i].IsDying()) {
                lstDyingChrs[i].KillCharacter();
                bFoundDeadChr = true;
            }
        }

        lstDyingChrs.Clear();

        return bFoundDeadChr;
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
