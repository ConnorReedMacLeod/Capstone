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

    public override void ApplicationEffect() {

        //Ensure that we're monitoring when the character on our position changes so we can update the soul affect we apply to the active character
        posTarget.subCharacterOnPositionChanged.Subscribe(cbOnChrOnPositionChanged);

        ChrEnteredPosition();
    }

    public override void RemoveEffect() {

        ChrLeftPosition();
    }

    public void ChrEnteredPosition() {
        if(chrOnPosition != null) {
            Debug.Log("Adding bunker effect to " + chrOnPosition.sName + " on " + posTarget.ToString());
            //Make a Permanent SoulChangeDefense, and save a reference to it, so it can be removed later
            soulChangeDefense = new SoulChangeDefense(chrSource, chrOnPosition, skillSource, nDefenseBuff);
            chrOnPosition.soulContainer.ApplySoul(soulChangeDefense);

        } else {
            Debug.Log("Tried to add bunker effect, but no character moved onto " + posTarget.ToString());
        }
    }

    public void ChrLeftPosition() {
        if(soulChangeDefense != null) {
            Debug.Log("Removing bunker soul effect from " + chrOnPosition.sName + " on " + posTarget.ToString());
            //We have an active soulChangeDefense reference floating around on some character

            //Remove the soulChangeDefense from the character it's already on
            soulChangeDefense.chrTarget.soulContainer.RemoveSoul(soulChangeDefense);

            soulChangeDefense = null;
        } else {
            Debug.Log("Tried to remove bunker effect, but no character was previously on " + posTarget.ToString());
        }
    }

    public void cbOnChrOnPositionChanged(Object target, params object[] args) {
        //Remove the soul effect from the leaving character
        ChrLeftPosition();

        //Add the soul effect to the incoming character
        ChrEnteredPosition();
    }

    public SoulPositionBunker(SoulPositionBunker other, Position _posTarget = null) : base(other, _posTarget) {

        nDefenseBuff = other.nDefenseBuff;

    }
}
