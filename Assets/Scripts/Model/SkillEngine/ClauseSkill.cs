using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A clause that is tied to a particular skill
//  (possible a side effect of a skill, or a direct
//    targetted effect of the skill)
public abstract class ClauseSkill : Clause {

    public Skill skill;

    public ClauseSkill(Skill _skill) {
        skill = _skill;
    }
}
