using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All this really needs to do is add a chrTarget field
public class SoulPosition : Soul {

    public Position posTarget;     //A reference to the Position this soul effect is applied to

    public Chr chrOnPosition {
        get {
            return posTarget.chrOnPosition;
        }
    }

    public SoulPosition(Chr _chrSource, Position _posTarget, Skill _skillSource) : base(_chrSource, _skillSource) {

        posTarget = _posTarget;

    }

    public SoulPosition(SoulPosition soulToCopy, Position _posTarget = null) : base(soulToCopy) {

        if(_posTarget != null) {
            //If a Target was provided, then we'll use that
            posTarget = _posTarget;
        } else {
            //Otherwise, just copy from the other object
            posTarget = soulToCopy.posTarget;
        }

    }
}