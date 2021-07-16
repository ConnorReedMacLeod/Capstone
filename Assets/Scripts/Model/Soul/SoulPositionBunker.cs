using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulPositionBunker : SoulPosition {

    public int nDefenseBuff;

    public SoulChangeDefense soulChangeDefense;

    public SoulPositionBunker(Chr _chrSource, Position _posTarget, Skill _skillSource) : base(_chrSource, _posTarget, _skillSource) {

        sName = "Bunker";

        nDefenseBuff = 15;

        bVisible = true;
        bDuration = true;
        pnMaxDuration = new Property<int>(12);
    }



    //Initialize with a non-empty list of Soul effects that you want to maintain on the character
    // that is currently on this position
    public override List<SoulChr> GetSoulToApplyToChrOnPosition() {
        return new List<SoulChr>() {
            new SoulChangeDefense(chrSource, chrOnPosition, skillSource, nDefenseBuff)
        };
    }


    public SoulPositionBunker(SoulPositionBunker other, Position _posTarget = null) : base(other, _posTarget) {

        nDefenseBuff = other.nDefenseBuff;

    }

}
