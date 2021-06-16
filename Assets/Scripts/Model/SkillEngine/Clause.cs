using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clauses store a set of executables that will be pushed onto the stack
/// once the Clause is on top of the stack.  This means that any replacement effects
/// or triggers that would interact with any of the stored executables 
///  will only take effect if they are present when the clause is evaluated
/// </summary>

public abstract class Clause {

    public enum TargetType { CHR, PLAYER, SKILL, SPECIAL };

    public TargetType targetType;

    public Skill skill;

    //Can this be submitted as a valid selection?
    public abstract bool IsSelectable(SelectionSerializer.SelectionInfo SelectionInfo);

    //Does the proposed targetting result in a non-null skill effect?
    public abstract bool HasFinalTarget(SelectionSerializer.SelectionInfo selectionInfo);

    public abstract string GetDescription();
    public abstract void Execute();

    //TODO - make a generic list of tags for qualitative effects (fire/teamwork/combo/etc.)

    public SelectionSerializer.SelectionInfo GetSelectionInfo() {
        //Ask the targetting type of the skill to fetch the selection information we should be using to determine our targets
        return skill.type.GetSelectionInfo();
    }

    public Clause(Skill _skill) {
        skill = _skill;
    }

}
