﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MatchResult {

    public enum RESULT { UNFINISHED, DRAW, DECISIVE };

    public abstract RESULT GetResult();

}

public class MatchResultUnfinished : MatchResult {

    public override RESULT GetResult() {
        return RESULT.UNFINISHED;
    }

    public override string ToString() {
        return "Match Result: Unfinished";
    }

}

public class MatchResultDraw : MatchResult {

    public override RESULT GetResult() {
        return RESULT.DRAW;
    }

    public override string ToString() {
        return "Match Result: Draw";
    }

}

public class MatchResultDecisive : MatchResult {

    public int nPlayerIDWinner;

    public MatchResultDecisive(int _nPlayerIDWinner) {
        nPlayerIDWinner = _nPlayerIDWinner;
    }

    public override RESULT GetResult() {
        return RESULT.DECISIVE;
    }

    public int GetWinner() {
        return nPlayerIDWinner;
    }

    public override string ToString() {
        return string.Format("MatchResult: Player {0} wins", nPlayerIDWinner);
    }
}
