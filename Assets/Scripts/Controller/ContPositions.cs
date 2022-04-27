using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


/// <summary>
/// Contains references to all positions and the interface for interacting with those positions.
/// Generally, don't try to interact with specific tiles and instead use the provided interface.
/// It's easy to forget that the bench positions are included in the pool of positions.
/// </summary>
public class ContPositions : Singleton<ContPositions> {


    public const int nROWS = 3;
    public const int nCOLUMNS = 6;
    public const int nBENCHCOLUMNSPERTEAM = 1;

    public List<Position> lstAllPositions;

    public static int CoordsToIndex(Position.Coords coords) {
        return CoordsToIndex(coords.iColumn, coords.jRow);
    }

    public static int CoordsToIndex(int iColumn, int jRow) {
        return iColumn * nROWS + jRow;
    }

    public static Position.Coords IndexToCoords(int i) {
        return new Position.Coords(i / nROWS, i % nROWS);
    }

    public Position GetPosition(Position.Coords coords) {
        return GetPosition(coords.iColumn, coords.jRow);
    }

    public Position GetPosition(int iColumn, int jRow) {
        return lstAllPositions[CoordsToIndex(iColumn, jRow)];
    }

    public Player GetPlayerOwnerOfPosition(Position pos) {
        if(pos.iColumn < nCOLUMNS / 2) return Match.Get().arPlayers[0];
        else return Match.Get().arPlayers[1];
    }

    public bool IsSameOwnerOfPosition(Position pos1, Position pos2) {
        return (pos1.iColumn < nCOLUMNS / 2) == (pos2.iColumn < nCOLUMNS / 2);
    }

    public bool IsDiffOwnerOfPosition(Position pos1, Position pos2) {
        return !IsSameOwnerOfPosition(pos1, pos2);
    }

    //Section for various Positional queries

    public List<Position> GetPositionsOfTypeForPlayer(Position.POSITIONTYPE postype, Player plyr) {

        int iColumn = 0;

        if(plyr.id == 0) {
            iColumn = (int)postype;
        } else {
            iColumn = nCOLUMNS - (int)postype - 1;
        }

        List<Position> lstColumn = new List<Position>();

        //Add each entry of the desired row
        for(int jRow = 0; jRow < nROWS; jRow++) {
            lstColumn.Add(GetPosition(iColumn, jRow));
        }

        return lstColumn;

    }

    public List<Chr> GetChrsInPositions(List<Position> lstPos) {

        List<Chr> lstChrs = new List<Chr>();

        for(int i = 0; i < lstPos.Count; i++) {
            if(lstPos[i].chrOnPosition != null) {
                lstChrs.Add(lstPos[i].chrOnPosition);
            }
        }

        return lstChrs;
    }

    //Common Allied Selections
    public List<Position> GetAlliedBenchPositions(Player plyr) {
        return GetPositionsOfTypeForPlayer(Position.POSITIONTYPE.BENCH, plyr);
    }
    public List<Chr> GetAlliedBenchChrs(Player plyr) {
        return GetChrsInPositions(GetAlliedBenchPositions(plyr));
    }

    public List<Position> GetAlliedBacklinePositions(Player plyr) {
        return GetPositionsOfTypeForPlayer(Position.POSITIONTYPE.BACKLINE, plyr);
    }
    public List<Chr> GetAlliedBacklineChrs(Player plyr) {
        return GetChrsInPositions(GetAlliedBacklinePositions(plyr));
    }

    public List<Position> GetAlliedFrontlinePositions(Player plyr) {
        return GetPositionsOfTypeForPlayer(Position.POSITIONTYPE.FRONTLINE, plyr);
    }
    public List<Chr> GetAlliedFrontlineChrs(Player plyr) {
        return GetChrsInPositions(GetAlliedFrontlinePositions(plyr));
    }

    public List<Position> GetInPlayAlliedPositions(Player plyr) {
        return GetAlliedFrontlinePositions(plyr).Concat(GetAlliedBacklinePositions(plyr)).ToList();
    }
    public List<Chr> GetInPlayAlliedChrs(Player plyr) {
        return GetChrsInPositions(GetInPlayAlliedPositions(plyr));
    }


    //Common Enemy Queries
    public List<Position> GetEnemyBenchPositions(Player plyr) {
        return GetPositionsOfTypeForPlayer(Position.POSITIONTYPE.BENCH, plyr.GetEnemyPlayer());
    }
    public List<Chr> GetEnemyBenchChrs(Player plyr) {
        return GetChrsInPositions(GetEnemyBenchPositions(plyr.GetEnemyPlayer()));
    }

    public List<Position> GetEnemyBacklinePositions(Player plyr) {
        return GetPositionsOfTypeForPlayer(Position.POSITIONTYPE.BACKLINE, plyr.GetEnemyPlayer());
    }
    public List<Chr> GetEnemyBacklineChrs(Player plyr) {
        return GetChrsInPositions(GetEnemyBacklinePositions(plyr.GetEnemyPlayer()));
    }

    public List<Position> GetEnemyFrontlinePositions(Player plyr) {
        return GetPositionsOfTypeForPlayer(Position.POSITIONTYPE.FRONTLINE, plyr.GetEnemyPlayer());
    }
    public List<Chr> GetEnemyFrontlineChrs(Player plyr) {
        return GetChrsInPositions(GetEnemyFrontlinePositions(plyr.GetEnemyPlayer()));
    }

    public List<Position> GetInPlayEnemyPositions(Player plyr) {
        return GetEnemyFrontlinePositions(plyr).Concat(GetEnemyBacklinePositions(plyr.GetEnemyPlayer())).ToList();
    }
    public List<Chr> GetInPlayEnemyChrs(Player plyr) {
        return GetChrsInPositions(GetInPlayEnemyPositions(plyr.GetEnemyPlayer()));
    }


    //Relative Positional Queries

    public List<Position> GetBesidePositions(Position pos) {
        List<Position> lstBesidePositions = new List<Position>();

        if(pos.jRow > 0) {
            lstBesidePositions.Add(GetPosition(pos.iColumn, pos.jRow - 1));
        }
        if(pos.jRow < nROWS - 1) {
            lstBesidePositions.Add(GetPosition(pos.iColumn, pos.jRow + 1));
        }

        return lstBesidePositions;
    }

    public Position GetBehindPosition(Position pos) {

        //Can only get positions behind the frontline
        if(pos.positiontype == Position.POSITIONTYPE.FRONTLINE) {

            //For the 0th Player (on the left), reduce the index by 1
            if(GetPlayerOwnerOfPosition(pos).id == 0) {
                return GetPosition(pos.iColumn - 1, pos.jRow);
            } else {
                //The other player (on the right), increases the index by 1 to move further right
                return GetPosition(pos.iColumn + 1, pos.jRow);
            }
        } else {
            return null;
        }
    }

    public Position GetInFrontPosition(Position pos) {

        //Can only get positions in front if not on the bench
        if(pos.positiontype != Position.POSITIONTYPE.BENCH) {

            //For the 0th Player (on the left), increase the index by 1
            if(GetPlayerOwnerOfPosition(pos).id == 0) {
                return GetPosition(pos.iColumn + 1, pos.jRow);
            } else {
                //The other player (on the right), decreases the index by 1 to move back to the left
                return GetPosition(pos.iColumn - 1, pos.jRow);
            }
        } else {
            return null;
        }
    }

    public List<Position> GetAdjacentPositions(Position pos) {
        List<Position> lstAdjacentPosition = GetBesidePositions(pos).ToList();

        lstAdjacentPosition.Add(GetBehindPosition(pos));

        lstAdjacentPosition.Add(GetInFrontPosition(pos));

        lstAdjacentPosition.Add(pos);

        return lstAdjacentPosition;
    }



    //Interface for modifying Positions

    //Initially sets the position of the character (so the character didn't previously have any position, 
    //   and no movement-triggers will triggered.  Also, can set to any in-play or bench position.
    public void InitChrToPosition(Chr chr, Position pos) {

        if(pos == null) {
            Debug.LogError("Cannot initialize a character to a null position");
            return;
        }

        if(chr.position != null) {
            Debug.LogError("Cannot initialize a character's position when they already have a non-null position");
            return;
        }

        pos.SetChrOnPosition(chr);

        chr.SetPosition(pos);

        pos.view.UpdateChrOnPositionToHere();
    }

    //When a character dies or is otherwise completely removed from the playing space, then this will 
    //  completely remove them (without triggering any movement triggers)
    public void DeleteChrFromPosition(Chr chr) {

        chr.position.SetChrOnPosition(null);

        chr.SetPosition(null);

        Debug.Log("TODO - delete the game object (maybe?) associated with the character");
    }

    // Triggers all relevent triggers for a movement of characters between the two consumed positions
    public void TriggerMovementTriggers(Position posStarting, Position posEnding) {

        Chr chrMoved = posEnding.chrOnPosition;
        Chr chrSwappedWith = posStarting.chrOnPosition;


        //Send all notifications out for affected characters and positions
        // Sending in reverse order of intended execution so that events placed on the stack
        //  will execute as expected

        //Note that the character will always send out generic subChrLeftAnyPosition/subEnteredAnyPosition updates,
        //  but will need to consult its position to determine if it should send out Bench/non-Bench updates.  

        //Let the positions broadcast that a new character has entered them 
        posEnding.subChrEnteredPosition.NotifyObs(chrMoved);
        posStarting.subChrEnteredPosition.NotifyObs(chrSwappedWith);


        //Send position-entering updates for the character we swapped with (if there is one)
        if (chrSwappedWith != null) {
            if(posStarting.positiontype != Position.POSITIONTYPE.BENCH) {
                chrSwappedWith.subEnteredInPlayPosition.NotifyObs(posStarting);
            } else if (posEnding.positiontype != Position.POSITIONTYPE.BENCH && posStarting.positiontype == Position.POSITIONTYPE.BENCH) {
                //If this character moved from a non-bench position to a bench position, then we'll throw the appropriate update
                chrSwappedWith.subEnteredBench.NotifyObs(posStarting);
            }

            chrSwappedWith.subEnteredAnyPosition.NotifyObs(posStarting);
        }
        //Send position-entering updates for the primary character we moved
        if (posEnding.positiontype != Position.POSITIONTYPE.BENCH) {
            chrMoved.subEnteredInPlayPosition.NotifyObs(posEnding);
        } else if (posStarting.positiontype != Position.POSITIONTYPE.BENCH && posEnding.positiontype == Position.POSITIONTYPE.BENCH) {
            //If this character moved from a non-bench position to a bench position, then we'll throw the appropriate update
            chrMoved.subEnteredBench.NotifyObs(posEnding);
        }
        chrMoved.subEnteredAnyPosition.NotifyObs(posEnding);

        //Let the positions broadcast that the character that was on them has left
        posEnding.subChrLeftPosition.NotifyObs(chrSwappedWith);
        posStarting.subChrLeftPosition.NotifyObs(chrMoved);

        
        //Send position-leaving updates for the character we swapped with (if there is one)
        if (chrSwappedWith != null) {
            if (posEnding.positiontype != Position.POSITIONTYPE.BENCH) {
                chrSwappedWith.subLeftInPlayPosition.NotifyObs(posEnding);
            } else if (posEnding.positiontype == Position.POSITIONTYPE.BENCH && posStarting.positiontype != Position.POSITIONTYPE.BENCH) {
                //If this character moved from a bench position to a non-bench position, then we'll throw the appropriate update
                chrSwappedWith.subLeftBench.NotifyObs(posEnding);
            }

            chrSwappedWith.subLeftAnyPosition.NotifyObs(posEnding);
        }
        //Send position-leaving updates for the primary character we moved
        if (posStarting.positiontype != Position.POSITIONTYPE.BENCH) {
            chrMoved.subLeftInPlayPosition.NotifyObs(posStarting);
        } else if (posStarting.positiontype == Position.POSITIONTYPE.BENCH && posEnding.positiontype != Position.POSITIONTYPE.BENCH) {
            //If this character moved from a bench position to a non-bench position, then we'll throw the appropriate update
            chrMoved.subLeftBench.NotifyObs(posStarting);
        }
        chrMoved.subLeftAnyPosition.NotifyObs(posStarting);

    }

    //Moves a character from an in-play position to an unoccupied in-play position
    public void MoveChrToPosition(Chr chr, Position pos) {

        Position posStarting = chr.position;

        if(posStarting == null) {
            Debug.LogError("Can't move a character that isn't already in a position - use InitChrToPosition instead");
            return;
        }

        if (posStarting == pos) {
            Debug.LogErrorFormat("{0} is already in position {1} - no need to do any switching", chr, pos);
        }

        if (IsDiffOwnerOfPosition(posStarting, pos)) {
            Debug.LogError("Can't move to an opponent's position");
            return;
        }

        if (chr.position.positiontype == Position.POSITIONTYPE.BENCH) {
            Debug.LogError("Can't move a character who is starting on the bench");
            return;
        }

        if (pos.positiontype == Position.POSITIONTYPE.BENCH) {
            Debug.LogError("Can't standardly move to a bench position");
            return;
        }

        if(pos.chrOnPosition != null) {
            Debug.LogError("Can't move " + chr.sName + " to " + pos.ToString() + " since it's occupied by " + pos.chrOnPosition.sName);
            return;
        }

        //Vacate the current space
        if(posStarting != null) posStarting.SetChrOnPosition(null);

        //Place this character on the target position
        pos.SetChrOnPosition(chr);
        chr.SetPosition(pos);

        //Send updates out for all affected character/positions
        TriggerMovementTriggers(pos, posStarting);

    }

    // Swaps the characters in two positions (optionally can be empty positions).
    private void SwapPositions(Position pos1, Position pos2) {

        if(IsDiffOwnerOfPosition(pos1, pos2)) {
            Debug.LogError("Can't move to an opponent's position");
            return;
        }

        Chr chr1 = pos1.chrOnPosition;
        Chr chr2 = pos2.chrOnPosition;

        //Update both positions
        pos1.SetChrOnPosition(chr2);
        pos2.SetChrOnPosition(chr1);


        //Update both characters
        if(chr1 != null) chr1.SetPosition(pos2);
        if(chr2 != null) chr2.SetPosition(pos1);
    }

    
    //Switches a character to another different allied position (and if there's already a character in that position, 
    //  then that character will move to the consumed character's starting position)
    public void SwitchChrToPosition(Chr chr, Position pos) {

        if(chr.position == pos) {
            Debug.LogErrorFormat("{0} is already in position {1} - no need to do any switching", chr, pos);
        }

        if(IsDiffOwnerOfPosition(chr.position, pos)) {
            Debug.LogError("Can't move to an opponent's position");
            return;
        }

        if(pos.positiontype == Position.POSITIONTYPE.BENCH) {
            Debug.LogError("Can't standardly swap to a bench position");
            return;
        }

        Chr chrSwappingWith = pos.chrOnPosition;

        if(chrSwappingWith == chr) {
            Debug.Log("Swapping " + chr.sName + " with themselves");
        }

        Position posStarting = chr.position;

        SwapPositions(pos, posStarting);

        //Send updates out for all affected character/positions
        TriggerMovementTriggers(pos, posStarting);

    }


    public void PrintAllPositions() {
        Debug.Log("Contents of Positions:");
        for(int i = 0; i < nCOLUMNS; i++) {
            for(int j = 0; j < nROWS; j++) {
                string sCharName = "Empty";

                if(GetPosition(i, j).chrOnPosition != null) sCharName = GetPosition(i, j).chrOnPosition.sName;

                Debug.Log(GetPosition(i, j).ToString() + ": " + sCharName);
            }
        }

    }

    public void ConfirmValidPositionSetup() {

        for(int iColumn = 0; iColumn < nCOLUMNS; iColumn++) {
            for(int jRow = 0; jRow < nROWS; jRow++) {
                Position pos = GetPosition(iColumn, jRow);
                Debug.Assert(pos.jRow == jRow);
                Debug.Assert(pos.iColumn == iColumn);

            }
        }

    }

    public override void Init() {

        for(int i = 0; i < lstAllPositions.Count; i++) {
            lstAllPositions[i].Start();
        }

        ConfirmValidPositionSetup();
    }



}
