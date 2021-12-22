using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This defines a clause that takes effect as part of an activated skill
//  It will draw input from the top of the NetworkReceiver buffer and execute
//  its effect by passing that input to ClauseEffect
public abstract class ClauseSkillSelection : ClauseSkill {

    

    public override void Execute() {
        //Grab the stored selections for the current input, and pass it to the overrideable ClauseEffect where
        // the abilities can customize what they will do

        ClauseEffect((InputSkillSelection)NetworkReceiver.Get().GetCurMatchInput());

    }

    //Let any extending clause instance decide what they want to do for their clause effect
    public abstract void ClauseEffect(InputSkillSelection selections);


    public ClauseSkillSelection(Skill _skill) : base(_skill) {

    }
}
