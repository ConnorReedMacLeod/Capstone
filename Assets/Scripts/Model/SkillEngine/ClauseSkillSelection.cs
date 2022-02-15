using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This defines a clause that takes effect as part of used skill
//  It will draw input from the NetworkReceiver (from the top usually for 
//  standard activated skills, but potentially from older inputs if we're using
//  a channeled delayed skill).  We then pass that input to ClauseEffect to execute
public abstract class ClauseSkillSelection : ClauseSkill {

    

    public override void Execute() {
        
        //Grab the stored selections input (as defined by our skill's usage type), and pass it to the overrideable ClauseEffect where
        // the abilities can customize what they will do

        ClauseEffect(skill.typeUsage.GetUsedSelections());

    }

    //Let any extending clause instance decide what they want to do for their clause effect
    public abstract void ClauseEffect(InputSkillSelection selections);


    public ClauseSkillSelection(Skill _skill) : base(_skill) {

    }
}
