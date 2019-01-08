using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulSmokeCover : Soul {

    public int nBaseDuration;

    public SoulChangePower soulChangePower;

    public void OnDeclareBlocker() {
        //When we block, dispel this effect
        soulContainer.RemoveSoul(this);
    }

    public SoulSmokeCover(Chr _chrSource, Chr _chrTarget) : base(_chrSource, _chrTarget) {

        sName = "SmokeCover";

        bVisible = true;
        bDuration = true;
        pnMaxDuration = new Property<int>(4);

        lstTriggers = new List<TriggerEffect>() {

            new TriggerEffect() {
                sub = ExecBecomeBlocker.subAllPostTrigger,
                cb = (target, args) => {
                    //Check which character is about to be blocking
                    Chr chrSource = ((ExecBecomeBlocker)args[0]).chrTarget;

                    OnDeclareBlocker();
                }
            }
        };

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
                execReplace = (Executable exec) => new ExecNull()

            }
        };

    }
}
