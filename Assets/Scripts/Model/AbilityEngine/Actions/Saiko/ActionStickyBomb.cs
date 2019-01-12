using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionStickyBomb : Action {

    public int nInitialDamage;

    public ActionStickyBomb(Chr _chrOwner) : base(1, _chrOwner) {//number of target arguments

        //We don't need to target anything, since we always deal damage to everyone
        arArgs[0] = new TargetArgChr((own, tar) => true); 

        sName = "StickyBomb";
        type = ActionTypes.TYPE.ACTIVE;

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 1, 0, 0, 1 });

        nCd = 6;
        nFatigue = 3;
        nActionCost = 1;

        sDescription = "Deal 5 damage to target character, then deal 30 damage to that character at the end of turn";

        nInitialDamage = 5;

        SetArgOwners();
    }

    override public void Execute() {
        //Cast the first target to be a character
        Chr chrTarget = ((TargetArgChr)arArgs[0]).chrTar;

        stackClauses.Push(new Clause() {
            fExecute = () => {

                //Deal initial damage to the target
                Damage dmgToDeal = new Damage(chrSource, chrTarget, nInitialDamage);

                ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                    chrSource = this.chrSource,
                    chrTarget = chrTarget,
                    dmg = dmgToDeal,

                    fDelay = 1.0f,
                    sLabel = chrTarget.sName + " got a bomb thrown at them"
                });

                //Apply the stickybomb Soul effect to the target
                ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
                    chrSource = this.chrSource,
                    chrTarget = chrTarget,

                    funcCreateSoul = (Chr _chrSource, Chr _chrTarget) => {
                        return new SoulStickyBomb(_chrSource, _chrTarget);
                    },

                    fDelay = 1.0f,
                    sLabel = chrTarget.sName + " is stuck with the bomb"

                });
            }
        });


        //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;
        base.Execute();
    }

}