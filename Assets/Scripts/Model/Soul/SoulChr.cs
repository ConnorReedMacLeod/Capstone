using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All this really needs to do is add a chrTarget field
public class SoulChr : Soul {

    public Chr chrTarget;     //A reference to the character this soul effect is applied to

    public SoulChr(Chr _chrSource, Chr _chrTarget, Skill _skillSource) : base(_chrSource, _skillSource) {

        chrTarget = _chrTarget;

    }

    public SoulChr(SoulChr soulToCopy, Chr _chrTarget = null) : base(soulToCopy) {

        if(_chrTarget != null) {
            //If a Target was provided, then we'll use that
            chrTarget = _chrTarget;
        } else {
            //Otherwise, just copy from the other object
            chrTarget = soulToCopy.chrTarget;
        }

    }
}
