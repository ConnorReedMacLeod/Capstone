using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position : MonoBehaviour {

    public enum POSITIONTYPE { BENCH, BACKLINE, FRONTLINE };

    public POSITIONTYPE positiontype;

    public delegate Position FuncGetPosition(Chr chr);

    public bool bStarted;

    public int iColumn;
    public int jRow;

    public Chr chrOnPosition;

    public SoulContainerPosition soulContainer;

    public Subject subChrLeftPosition = new Subject();
    public Subject subChrEnteredPosition = new Subject();
    public Subject subSoulApplied = new Subject();
    public Subject subSoulRemoved = new Subject();
    
    public Subject subBecomesTargettable = new Subject(); // When a skill that is choosing targets can target this character
    public Subject subEndsTargettable = new Subject(); // When the skill that could target this character stops its targetting process

    public override string ToString() {
        return string.Format("({0},{1})", iColumn, jRow);
    }

    public override bool Equals(object other) {
        if (other.GetType() != this.GetType()) return false;
        Position posOther = (Position)other;
        return (this.iColumn == posOther.iColumn) == (this.jRow == posOther.jRow);
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

    }


    public void Start() {

        if(bStarted == true) return;
        bStarted = true;

        subChrEnteredPosition = new Subject();
        subChrLeftPosition = new Subject();
        subSoulApplied = new Subject();
        subSoulRemoved = new Subject();

    }


    //Defines the default starting positions for characters (the standard triangle setup)
    public static Position[][] arDefaultStartingPositions = new Position[][] {
        //Player 0:
        new Position[]{
            new Position(1, 0),
            new Position(2, 1),
            new Position(1, 2)
        },

        //Player 1:
        new Position[] {
            new Position(4, 0),
            new Position(3, 1),
            new Position(4, 2)
        }
    };

    public static Position GetDefaultPosition(int iPlayer, int iChr) {
        return arDefaultStartingPositions[iPlayer][iChr];
    }

}
