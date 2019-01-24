using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionStickyBomb : Action {

    public Damage dmg;
    public int nBaseDamage;

    public ActionStickyBomb(Chr _chrOwner) : base(1, _chrOwner) {//number of target arguments

        //We don't need to target anything, since we always deal damage to everyone
        arArgs[0] = new TargetArgChr((own, tar) => true); 

        sName = "StickyBomb";
        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 1, 0, 0, 1 });

        nCd = 6;
        nFatigue = 3;
        nActionCost = 1;

        nBaseDamage = 5;
        //Create a base Damage object that this action will apply
        dmg = new Damage(this.chrSource, null, nBaseDamage);

        sDescription = "Deal 5 damage to target character, then deal 30 damage to that character at the end of turn";

        

        SetArgOwners();
    }

    override public void Execute() {
        //Cast the first target to be a character
        Chr tar = ((TargetArgChr)arArgs[0]).chrTar;

        stackClauses.Push(new Clause() {
            fExecute = () => {

                //Make a copy of the damage object to give to the executable
                Damage dmgToApply = new Damage(dmg);
                //Give the damage object its target
                dmgToApply.SetChrTarget(tar);

                ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                    chrSource = this.chrSource,
                    chrTarget = tar,
                    dmg = dmgToApply,

                    fDelay = 1.0f,
                    sLabel = tar.sName + " got a bomb thrown at them"
                });

                //Apply the stickybomb Soul effect to the target
                ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
                    chrSource = this.chrSource,
                    chrTarget = tar,

                    funcCreateSoul = (Chr _chrSource, Chr _chrTarget) => {
                        return new SoulStickyBomb(_chrSource, _chrTarget);
                    },

                    fDelay = 1.0f,
                    sLabel = tar.sName + " is stuck with the bomb"

                });
            }
        });


        //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;
        base.Execute();
    }

}