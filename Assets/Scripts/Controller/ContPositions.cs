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

    public List<List<Position>> lstAllPositions;



    public Player GetPlayerOwnerOfPosition(Position pos) {
        if(pos.iColumn < nCOLUMNS / 2) return Player.GetTargetByIndex(0);
        else return Player.GetTargetByIndex(1);
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

        return lstAllPositions[iColumn];

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



    //Interface for modifying Positions

    public void MoveChrToPosition(Chr chr, Position pos) {

        if(IsDiffOwnerOfPosition(chr.position, pos)) {
            Debug.LogError("Can't move to an opponent's position");
            return;
        }

        if(pos.positiontype == Position.POSITIONTYPE.BENCH) {
            Debug.LogError("Can't standardly move to a bench position");
            return;
        }

        if(pos.chrOnPosition != null) {
            Debug.LogError("Can't move " + chr.sName + " to " + pos.ToString() + " since it's occupied by " + pos.chrOnPosition.sName);
            return;
        }

        Position posStarting = chr.position;

        //Vacate the current space
        posStarting.SetChrOnPosition(null);

        //Place this character on the target position
        pos.SetChrOnPosition(chr);
        chr.UpdatePosition(pos);


        //Send updates out for all affected character/positions (in reverse order so they take effect in chronological order)
        chr.subPositionChanged.NotifyObs();
        pos.subCharacterOnPositionChanged.NotifyObs();

        posStarting.subCharacterOnPositionChanged.NotifyObs();

    }

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
        chr1.UpdatePosition(pos2);
        chr2.UpdatePosition(pos1);

    }

    public void SwitchChrToPosition(Chr chr, Position pos) {

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

        //Send all notifications out for affected characters and positions
        chr.subPositionChanged.NotifyObs();
        pos.subCharacterOnPositionChanged.NotifyObs();

        chrSwappingWith.subPositionChanged.NotifyObs();
        posStarting.subCharacterOnPositionChanged.NotifyObs();

    }

    public void SwitchChrToBench(Chr chr, Position pos) {

        if(IsDiffOwnerOfPosition(chr.position, pos)) {
            Debug.LogError("Can't move to an opponent's position");
            return;
        }

        if(pos.positiontype != Position.POSITIONTYPE.BENCH) {
            Debug.LogError("Can only select a bench-position to move to when bench-swapping");
            return;
        }

        if(chr.position.positiontype == Position.POSITIONTYPE.BENCH) {
            Debug.LogError("Can't swap to the bench when " + chr.sName + " is already on the bench");
            return;
        }

        Position posStarting = chr.position;

        SwapPositions(pos, posStarting);

        //TODO - figure out what bench triggers to put in here
        //  Will probably have to put in similar sub notifications as in the SwitchChr function
    }

    public override void Init() {

        lstAllPositions = new List<List<Position>>();
        for(int i = 0; i < nCOLUMNS; i++) {
            lstAllPositions[i] = new List<Position>();
            for(int j = 0; j < nROWS; j++) {
                lstAllPositions[i][j] = new Position(i, j);
            }
        }

    }



}
