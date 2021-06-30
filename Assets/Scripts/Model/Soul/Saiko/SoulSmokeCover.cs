using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulSmokeCover : SoulChr {

    public SoulSmokeCover(Chr _chrSource, Chr _chrTarget, Skill _skillSource) : base(_chrSource, _chrTarget, _skillSource) {

        sName = "SmokeCover";

        bVisible = true;
        bDuration = true;

        bRecoilWhenApplied = false;

        pnMaxDuration = new Property<int>(4);

        lstReplacements = new List<Replacement>() {
            new Replacement() {

                //The list of replacement effects we'll include ourselves in
                lstExecReplacements = ExecDealDamage.lstAllFullReplacements,

                //Note that the parameter type is the generic Executable
                // - should cast to the proper type if further checking is required
                shouldReplace = (Executable exec) => {
                        Debug.Assert(typeof(ExecDealDamage) == exec.GetType());

                        //replace only if the damaged character will be the character this effect is on
                        return ((ExecDealDamage)exec).chrTarget == this.chrTarget;
                    },

                //Just replace the executable with a completely new null executable
                execReplace = (Executable exec) => new ExecNull(exec.chrSource)

            }
        };

    }

    public SoulSmokeCover(SoulSmokeCover other, Chr _chrTarget = null) : base(other) {
        if(_chrTarget != null) {
            //If a Target was provided, then we'll use that
            chrTarget = _chrTarget;
        } else {
            //Otherwise, just copy from the other object
            chrTarget = other.chrTarget;
        }
    }
}
