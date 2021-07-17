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

    public override string ToString() {
        return string.Format("({0},{1})", iColumn, jRow);
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

        subChrLeftPosition.Subscribe(cbOnChrLeft);
        subChrEnteredPosition.Subscribe(cbOnChrEntered);
    }


    public void cbOnChrLeft(Object target, params object[] args) {
        Debug.Log(ToString() + " has been notified that " + (Chr)target + " has left the position");
    }

    public void cbOnChrEntered(Object target, params object[] args) {
        Debug.Log(ToString() + " has been notified that " + (Chr)target + " has entered the position");
    }

}
