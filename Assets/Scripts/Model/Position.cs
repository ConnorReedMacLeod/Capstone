using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position {

    public enum POSITIONTYPE { BENCH, BACKLINE, FRONTLINE };

    public POSITIONTYPE positiontype;

    public int iColumn;
    public int jRow;

    public Chr chrOnPosition;

    public Subject subCharacterOnPositionChanged = new Subject();

    public override string ToString() {
        return string.Format("({0},{1}", iColumn, jRow);
    }

    public void SetChrOnPosition(Chr _chrOnPosition) {

        if(chrOnPosition == _chrOnPosition) return;

        chrOnPosition = _chrOnPosition;
    }

    public void InitPositionType() {

        if(iColumn == 0 || iColumn == 5) {
            positiontype = POSITIONTYPE.BENCH;
        } else if(iColumn == 1 || iColumn == 4) {
            positiontype = POSITIONTYPE.BACKLINE;
        } else {
            positiontype = POSITIONTYPE.FRONTLINE;
        }

    }

    public bool IsAllyOwned(Player plyr) {
        return ContPositions.Get().GetPlayerOwnerOfPosition(this) == plyr;
    }

    public bool IsEnemyOwned(Player plyr) {
        return !IsAllyOwned(plyr);
    }

    public Position(int _iColumn, int _jRow) {

        iColumn = _iColumn;
        jRow = _jRow;

        InitPositionType();

    }
}
