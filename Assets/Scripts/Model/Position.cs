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

    [System.Serializable]
    public struct Coords {
        public int iColumn;
        public int jRow;

        public Coords(int _iColumn, int _jRow) {
            iColumn = _iColumn;
            jRow = _jRow;
        }

        public override bool Equals(object obj) {
            if (obj.GetType() != this.GetType()) return false;
            return (((Coords)obj).iColumn == this.iColumn) && (((Coords)obj).jRow == this.jRow);
        }
    }

    public static int SerializeCoords(Position.Coords coords) {
        return ContPositions.CoordsToIndex(coords);
    }

    public static Position.Coords UnserializeCoords(int nSerialized) {
        return ContPositions.IndexToCoords(nSerialized);
    }

    public override string ToString() {
        return string.Format("({0},{1}) ({2})", iColumn, jRow, positiontype);
    }

    public override bool Equals(object other) {
        if (other.GetType() != this.GetType()) return false;
        return this.coords.Equals(((Position)other).coords);
    }

    public void SetChrOnPosition(Chr _chrOnPosition) {

        if(chrOnPosition == _chrOnPosition) return;

        chrOnPosition = _chrOnPosition;

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

    public bool IsAllyOwned(Player plyr) {
        return ContPositions.Get().GetPlayerOwnerOfPosition(this) == plyr;
    }

    public bool IsEnemyOwned(Player plyr) {
        return !IsAllyOwned(plyr);
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


    //Defines the default starting coords of positions for characters (the standard triangle setup)
    public static Coords[][] arDefaultStartingPositions = new Coords[][] {
        //Player 0:
        new Coords[]{
            new Coords(1, 0),
            new Coords(2, 1),
            new Coords(1, 2)
        },

        //Player 1:
        new Coords[] {
            new Coords(4, 0),
            new Coords(3, 1),
            new Coords(4, 2)
        }
    };

    //Fetch the default position coords for this player
    public static Position.Coords GetDefaultPositionCoords(int iPlayer, int iChr) {
        return arDefaultStartingPositions[iPlayer][iChr];
    }

}
