using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position : MonoBehaviour {

    public enum POSITIONTYPE { BENCH, BACKLINE, FRONTLINE };

    public POSITIONTYPE positiontype;

    public delegate Position FuncGetPosition(Chr chr);

    public bool bStarted;

    public Coords coords;

    public int iColumn {
        get { return coords.iColumn; }
        set { coords.iColumn = value; }
    }

    public int jRow {
        get { return coords.jRow; }
        set { coords.jRow = value; }
    }

    public Chr chrOnPosition;

    public ViewPosition view;

    public SoulContainerPosition soulContainer;

    public Subject subChrLeftPosition = new Subject();
    public Subject subChrEnteredPosition = new Subject();
    public Subject subSoulApplied = new Subject();
    public Subject subSoulRemoved = new Subject();

    public Subject subBecomesTargettable = new Subject(); // When a skill that is choosing targets can target this character
    public Subject subEndsTargettable = new Subject(); // When the skill that could target this character stops its targetting process


    // Coords visualization

    // j\i  0    1    2  |  3    4    5
    //  0                |
    //  1                |
    //  2                |

    // Columns 0,5 are bench
    // Columns 1,4 are backline
    // Columns 2,3 are frontline


    [System.Serializable]
    public struct Coords {
        public int iColumn;
        public int jRow;

        public Coords(int _iColumn, int _jRow) {
            iColumn = _iColumn;
            jRow = _jRow;
        }

        public Coords(Coords other) {
            iColumn = other.iColumn;
            jRow = other.jRow;
        }

        public override bool Equals(object obj) {
            if(obj.GetType() != this.GetType()) return false;
            return (((Coords)obj).iColumn == this.iColumn) && (((Coords)obj).jRow == this.jRow);
        }

        public override string ToString() {
            return string.Format("({0},{1})", iColumn, jRow);
        }

    }

    public override string ToString() {
        return string.Format("({0},{1}) ({2})", iColumn, jRow, positiontype);
    }

    public string ToPrettyString() {
        string sPrettyColumn = "";
        string sPrettyRow = "";

        if(positiontype == POSITIONTYPE.BACKLINE) {
            sPrettyColumn = "Backline";
        } else if(positiontype == POSITIONTYPE.FRONTLINE) {
            sPrettyColumn = "Frontline";
        } else if(positiontype == POSITIONTYPE.BENCH) {
            sPrettyColumn = "Bench";
        }

        if(jRow == 0) {
            sPrettyRow = "Top";
        } else if(jRow == 1) {
            sPrettyRow = "Mid";
        } else if(jRow == 2) {
            sPrettyRow = "Bot";
        }

        return string.Format("{0}/{1}", sPrettyRow, sPrettyColumn);
    }

    public override bool Equals(object other) {
        if(other.GetType() != this.GetType()) return false;
        return this.coords.Equals(((Position)other).coords);
    }

    public void SetChrOnPosition(Chr _chrOnPosition) {

        if(chrOnPosition == _chrOnPosition) return;

        chrOnPosition = _chrOnPosition;

        view.UpdateChrOnPositionToHere();
    }

    public void InitPositionType() {

        if(coords.iColumn == 0 || coords.iColumn == 5) {
            positiontype = POSITIONTYPE.BENCH;
        } else if(coords.iColumn == 1 || coords.iColumn == 4) {
            positiontype = POSITIONTYPE.BACKLINE;
        } else {
            positiontype = POSITIONTYPE.FRONTLINE;
        }

    }

    public int PlyrIdOwnedBy() {
        if(coords.iColumn < 3) return 0;
        else return 1;
    }

    public Player PlyrOwnedBy() {
        return Match.Get().arPlayers[PlyrIdOwnedBy()];
    }

    public bool IsAllyOwned(Player plyr) {
        return ContPositions.Get().GetPlayerOwnerOfPosition(this) == plyr;
    }

    public bool IsEnemyOwned(Player plyr) {
        return !IsAllyOwned(plyr);
    }

    public bool IsActivePosition() {
        return positiontype != POSITIONTYPE.BENCH;
    }

    public bool IsBench() {
        return positiontype == POSITIONTYPE.BENCH;
    }

    public Position(int _iColumn, int _jRow) : this(new Coords(_iColumn, _jRow)) {

    }

    public Position(Coords _coords) {
        coords = _coords;

        InitPositionType();
    }


    public void Start() {

        if(bStarted == true) return;
        bStarted = true;

        InitPositionType();

        subChrEnteredPosition = new Subject();
        subChrLeftPosition = new Subject();
        subSoulApplied = new Subject();
        subSoulRemoved = new Subject();

        subBecomesTargettable = new Subject();
        subEndsTargettable = new Subject();

        view.Start();
    }

}
